using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Utilities;

internal static class SiteAssemblyInformationFactory
{
    internal static SiteAssemblyInformation GetAssemblyInformation(IEnumerable<Type> exportedTypes, IEnumerable<Attribute> customAttributes, string defaultCulture)
    {
        var controllersNames = GetControllerNames(exportedTypes);
        var views = GetViewCultures(exportedTypes, customAttributes, defaultCulture);

        return new SiteAssemblyInformation(controllersNames, views);
    }

    private static IReadOnlyCollection<CultureBasedView> GetViewCultures(IEnumerable<Type> exportedTypes, IEnumerable<Attribute> customAttributes, string defaultCulture)
    {
        var views = FindViews(exportedTypes, customAttributes);

        return views.Select(viewName =>
        {
            // The viewName does not have the .cshtml extension at this point,
            // so we can assume that a dot is for a specific culture.
            var parts = viewName.Split('.');

            if (parts.Length == 1 || parts.Length > 2)
            {
                return (viewName, defaultCulture);
            }

            return (parts[0], parts[1]);
        })
            .GroupBy(x => x.Item1)
            .Select(x => new CultureBasedView(x.Key, x.Select(y => y.Item2).ToArray()))
            .ToArray();
    }

    private static IReadOnlyCollection<string> GetControllerNames(IEnumerable<Type> exportedTypes)
    {
        return FindControllerTypes(exportedTypes)
            .Select(x =>
            {
                if (x.Name.EndsWith("Controller"))
                {
                    return x.Name[..^10];
                }

                return x.Name;
            })
            .ToArray();
    }

    private static IEnumerable<Type> FindControllerTypes(IEnumerable<Type> exportedTypes)
    {
        var controllerBaseType = typeof(Controller);

        return exportedTypes.Where(x => controllerBaseType.IsAssignableFrom(x));
    }

    private static IEnumerable<string> FindViews(IEnumerable<Type> exportedTypes, IEnumerable<Attribute> customAttributes)
    {
        var nonModelViewBaseType = typeof(RazorPage<object>);
        var actionRoutes = GetActionRoutes(exportedTypes);

        return customAttributes
            .OfType<RazorCompiledItemAttribute>()
            .Where(x =>
            {
                // We only accept Views that do not have a Model declared, which unfortunately means
                // accepting types that inherit from `RazorPage<object>`.
                // If someone has a view with `@model object`, it would slip through here, which is unwanted as it couldn't be rendered.
                if (!nonModelViewBaseType.IsAssignableFrom(x.Type))
                {
                    return false;
                }

                return actionRoutes.Any(v =>
                {
                    var viewPath = $"/Views/{v}";
                    var viewFilename = $"{viewPath}.cshtml";

                    if (viewFilename.Equals(x.Identifier, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                    // For example: /Views/Home/Index.es.cshtml => represents a localized view
                    if (x.Identifier.StartsWith(viewPath + ".") && x.Identifier.EndsWith(".cshtml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                    return false;
                });
            })
            .Select(x =>
            {
                // Remove /views/ from x.Identifier
                var identifier = x.Identifier[7..];

                // Remove .cshtml from identifier
                var index = identifier.LastIndexOf(".cshtml", StringComparison.Ordinal);
                if (index > 0)
                {
                    identifier = identifier[..index];
                }

                return identifier;
            })
            .ToArray();
    }

    private static string[] GetActionRoutes(IEnumerable<Type> exportedTypes)
    {
        return FindControllerTypes(exportedTypes)
            .SelectMany(controllerType => controllerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(IActionResult))
                .Select(method =>
                {
                    var controllerName = controllerType.Name;
                    var controller = controllerName.EndsWith("Controller") ? controllerName[..^10] : controllerName;

                    var template = method.GetCustomAttribute<RouteAttribute>()?.Template ??
                                   method.GetCustomAttribute<HttpGetAttribute>()?.Template;

                    // If the action has a templated route, use that.
                    if (!string.IsNullOrEmpty(template))
                    {
                        if (template.StartsWith("/", StringComparison.Ordinal))
                        {
                            return template[1..];
                        }

                        return $"{controller}/{template}";
                    }

                    // Otherwise, use the controller's and the action's name.


                    var action = method.Name;

                    return $"{controller}/{action}";
                }))
            .ToArray();
    }

}

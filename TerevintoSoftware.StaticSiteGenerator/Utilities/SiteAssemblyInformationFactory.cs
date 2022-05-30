using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Utilities;

internal static class SiteAssemblyInformationFactory
{
    internal static SiteAssemblyInformation GetAssemblyInformation(Assembly assembly)
    {
        var exportedTypes = assembly.GetExportedTypes();
        var customAttributes = assembly.GetCustomAttributes();

        return new SiteAssemblyInformation(FindControllerNames(exportedTypes), FindViews(exportedTypes, customAttributes));
    }

    private static IReadOnlyCollection<string> FindViews(IEnumerable<Type> exportedTypes, IEnumerable<Attribute> customAttributes)
    {
        var nonModelViewBaseType = typeof(RazorPage<object>);
        var actionRoutes = FindActionRoutes(exportedTypes);

        return customAttributes
            .OfType<RazorCompiledItemAttribute>()
            .Where(x => nonModelViewBaseType.IsAssignableFrom(x.Type) &&
                   actionRoutes.Any(v => x.Identifier == $"/Views/{v}.cshtml"))
            .Select(x =>
            {
                // remove /views/ from x.Identifier
                var identifier = x.Identifier[7..];

                // remove .cshtml from identifier
                var index = identifier.LastIndexOf(".cshtml", StringComparison.Ordinal);
                if (index > 0)
                {
                    identifier = identifier[..index];
                }

                return identifier;
            })
            .ToArray();
    }

    private static string[] FindActionRoutes(IEnumerable<Type> exportedTypes)
    {
        return FindControllerTypes(exportedTypes)
            .SelectMany(controllerType => controllerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(IActionResult))
                .Select(method =>
                {
                    // If the action has a templated route, use that.
                    var template = method.GetCustomAttribute<RouteAttribute>()?.Template ??
                                   method.GetCustomAttribute<HttpGetAttribute>()?.Template;

                    if (!string.IsNullOrEmpty(template))
                    {
                        if (template.StartsWith("/", StringComparison.Ordinal))
                        {
                            template = template[1..];
                        }

                        return template;
                    }

                    // Otherwise, use the controller's and the action's name.

                    var controllerName = controllerType.Name;
                    var controller = controllerName.EndsWith("Controller") ? controllerName[..^10] : controllerName;
                    var action = method.Name;

                    return $"{controller}/{action}";
                }))
            .ToArray();
    }

    private static IReadOnlyCollection<string> FindControllerNames(IEnumerable<Type> exportedTypes)
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
}

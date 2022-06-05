using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Models;
using TerevintoSoftware.StaticSiteGenerator.Services;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal class ViewCompilerService : IViewCompilerService
{
    private readonly MvcViewOptions _viewOptions;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEndpointProvider _endpointProvider;
    private readonly IHtmlFormatter _htmlFormatter;
    private readonly StaticSiteGenerationOptions _staticSiteGenerationOptions;

    public ViewCompilerService(IOptions<MvcViewOptions> viewOptions, ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider, IEndpointProvider endpointProvider, IHtmlFormatter htmlFormatter,
        StaticSiteGenerationOptions staticSiteGenerationOptions)
    {
        _viewOptions = viewOptions.Value;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
        _endpointProvider = endpointProvider;
        _htmlFormatter = htmlFormatter;
        _staticSiteGenerationOptions = staticSiteGenerationOptions;
    }

    public async Task<IEnumerable<ViewGenerationResult>> CompileViews(IEnumerable<CultureBasedView> viewsToRender)
    {
        var bag = new ConcurrentBag<ViewGenerationResult>();

        var views = viewsToRender.SelectMany(x => x.Cultures.Select(y => (viewName: x.ViewName, culture: y)));

#if DEBUG
        foreach (var view in views)
#else
        await Parallel.ForEachAsync(views, async (view, ct) =>
#endif
        {
            var (viewNameWithoutCulture, culture) = view;

            var viewName = viewNameWithoutCulture;

            if (culture != _staticSiteGenerationOptions.DefaultCulture)
            {
                viewName += "." + culture;
            }

            try
            {
                var html = await GetCompiledView(viewName, culture);

                html = _htmlFormatter.FixRelativeLinks(html, culture);

                bag.Add(new ViewGenerationResult(viewName, new GeneratedView(viewNameWithoutCulture + ".html", html, culture)));
            }
            catch (Exception ex)
            {
                bag.Add(new ViewGenerationResult(viewName, ex.Message));
            }
#if DEBUG           
        }
#else
        });
#endif

        return bag.ToArray();
    }

    private async Task<string> GetCompiledView(string viewName, string? culture)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = new DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };
        context.SetEndpoint(_endpointProvider.Endpoint);
        var routeData = context.GetRouteData();

        var cultureInfo = new CultureInfo(culture ?? _staticSiteGenerationOptions.DefaultCulture);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        
        var actionContext = new ActionContext(context, routeData, new ActionDescriptor());

        var viewEngine = _viewOptions.ViewEngines.First();
        var htmlHelperOptions = _viewOptions.HtmlHelperOptions;

        var view = viewEngine.FindView(actionContext, viewName, true).View;

        if (view == null)
        {
            throw new InvalidOperationException($"Unable to find view '{viewName}'");
        }

        var builder = new StringBuilder();

        using (var output = new StringWriter(builder))
        {
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()),
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                output,
                htmlHelperOptions);

            await view.RenderAsync(viewContext);
        }

        return builder.ToString();
    }
}

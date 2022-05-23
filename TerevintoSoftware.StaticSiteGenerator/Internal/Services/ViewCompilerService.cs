using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal class ViewCompilerService : IViewCompilerService
{
    private readonly MvcViewOptions _viewOptions;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public ViewCompilerService(IOptions<MvcViewOptions> viewOptions, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        _viewOptions = viewOptions.Value;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<ViewGenerationResult>> CompileViews(IEnumerable<string> viewsToRender)
    {
        var bag = new ConcurrentBag<ViewGenerationResult>();
        
        await Parallel.ForEachAsync(viewsToRender, async (viewName, ct) =>
        {
            try
            {
                var html = await GetCompiledView(viewName);

                bag.Add(new ViewGenerationResult(viewName, new GeneratedView(viewName + ".html", html)));
            }
            catch (Exception ex)
            {
                bag.Add(new ViewGenerationResult(viewName, ex.Message));
            }
        });

        return bag.ToArray();
    }

    private async Task<string> GetCompiledView(string viewName)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = new DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };

        var routeData = context.GetRouteData();
        routeData.Routers.Add(new RouteCollection());

        var actionContext = new ActionContext(context, routeData, new ControllerActionDescriptor());
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

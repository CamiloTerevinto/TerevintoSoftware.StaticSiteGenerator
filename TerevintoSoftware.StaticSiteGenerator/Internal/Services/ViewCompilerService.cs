using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal class ViewCompilerService : IViewCompilerService
{
    private readonly MvcViewOptions _viewOptions;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public ViewCompilerService(MvcViewOptions viewOptions, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        _viewOptions = viewOptions;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async IAsyncEnumerable<(string, string)> GenerateHtml(IEnumerable<string> viewsToRender)
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

        foreach (var viewName in viewsToRender)
        {
            //var paths = view.Split('/');
            //paths = paths.Take(paths.Length - 2).ToArray();
            //var folderPath = Path.Join(paths);

            //var filePath = Path.Join(folderPath, paths[^-1] + ".html");
            
            yield return (viewName + ".html", await GetCompiledView(actionContext, viewEngine, htmlHelperOptions, viewName));
        }
    }

    private async Task<string> GetCompiledView(ActionContext actionContext, IViewEngine viewEngine, HtmlHelperOptions htmlHelperOptions, string viewName)
    {
        var view = viewEngine.FindView(actionContext, viewName, true).View!;

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

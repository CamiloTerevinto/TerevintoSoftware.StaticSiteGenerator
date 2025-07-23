using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;

internal interface IViewRenderService
{
    Task<string> GetCompiledView(string viewName, string? culture);
}

internal class ViewRenderService(IOptions<MvcViewOptions> viewOptions, ITempDataProvider tempDataProvider,
    IActionContextFactory actionContextFactory, StaticSiteGenerationOptions staticSiteGenerationOptions) : IViewRenderService
{
    private readonly ITempDataProvider _tempDataProvider = tempDataProvider;
    private readonly IActionContextFactory _actionContextFactory = actionContextFactory;
    private readonly StaticSiteGenerationOptions _staticSiteGenerationOptions = staticSiteGenerationOptions;
    private readonly IViewEngine _viewEngine = viewOptions.Value.ViewEngines.First();
    private readonly HtmlHelperOptions _htmlHelperOptions = viewOptions.Value.HtmlHelperOptions;

    public async Task<string> GetCompiledView(string viewName, string? culture)
    {
        var splitted = viewName.Split('/');
        var controllerName = splitted[0];
        viewName = splitted[1];

        var actionContext = _actionContextFactory.Create(controllerName, viewName);

        var cultureInfo = new CultureInfo(culture ?? _staticSiteGenerationOptions.DefaultCulture);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        var view = _viewEngine.FindView(actionContext, viewName, true).View ?? throw new InvalidOperationException($"Unable to find view '{viewName}'");
        var builder = new StringBuilder();

        using (var output = new StringWriter(builder))
        {
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()),
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                output,
                _htmlHelperOptions);

            await view.RenderAsync(viewContext);
        }

        return builder.ToString();
    }
}

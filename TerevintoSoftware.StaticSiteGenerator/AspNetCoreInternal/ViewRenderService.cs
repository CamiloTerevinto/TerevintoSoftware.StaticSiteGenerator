using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;

internal interface IViewRenderService
{
    Task<string> GetCompiledView(string viewName, string? culture);
}

internal class ViewRenderService : IViewRenderService
{
    private readonly MvcViewOptions _viewOptions;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IActionContextFactory _actionContextFactory;
    private readonly StaticSiteGenerationOptions _staticSiteGenerationOptions;

    public ViewRenderService(IOptions<MvcViewOptions> viewOptions, ITempDataProvider tempDataProvider,
        IActionContextFactory actionContextFactory, StaticSiteGenerationOptions staticSiteGenerationOptions)
    {
        _viewOptions = viewOptions.Value;
        _tempDataProvider = tempDataProvider;
        _actionContextFactory = actionContextFactory;
        _staticSiteGenerationOptions = staticSiteGenerationOptions;
    }

    public async Task<string> GetCompiledView(string viewName, string? culture)
    {
        var actionContext = _actionContextFactory.Create();

        var cultureInfo = new CultureInfo(culture ?? _staticSiteGenerationOptions.DefaultCulture);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

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

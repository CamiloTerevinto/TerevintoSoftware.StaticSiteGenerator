using System.Collections.Concurrent;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Models;

namespace TerevintoSoftware.StaticSiteGenerator.Services;

internal interface IViewCompilerService
{
    Task<IEnumerable<ViewGenerationResult>> CompileViews(IEnumerable<CultureBasedView> viewsToRender);
}

internal class ViewCompilerService(IViewRenderService viewRenderService, IHtmlFormatter htmlFormatter,
    StaticSiteGenerationOptions staticSiteGenerationOptions) : IViewCompilerService
{
    private readonly IViewRenderService _viewRenderService = viewRenderService;
    private readonly StaticSiteGenerationOptions _staticSiteGenerationOptions = staticSiteGenerationOptions;
    private readonly IHtmlFormatter _htmlFormatter = htmlFormatter;

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
            var (viewNameWithoutCulture, currentCulture) = view;

            var viewName = viewNameWithoutCulture;

            if (currentCulture != _staticSiteGenerationOptions.DefaultCulture)
            {
                viewName += "." + currentCulture;
            }

            try
            {
                var html = await _viewRenderService.GetCompiledView(viewName, currentCulture);

                html = _htmlFormatter.FixRelativeLinks(html, currentCulture);

                bag.Add(new ViewGenerationResult(viewName, currentCulture, new GeneratedView(viewNameWithoutCulture + ".html", html)));
            }
            catch (Exception ex)
            {
                bag.Add(new ViewGenerationResult(viewName, currentCulture, ex.Message));
            }
#if DEBUG           
        }
#else
        });
#endif

        return bag.ToArray();
    }
}

using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Models;

namespace TerevintoSoftware.StaticSiteGenerator.Services;

internal interface IViewCompilerService
{
    Task<IEnumerable<ViewGenerationResult>> CompileViews(IEnumerable<CultureBasedView> viewsToRender);
}

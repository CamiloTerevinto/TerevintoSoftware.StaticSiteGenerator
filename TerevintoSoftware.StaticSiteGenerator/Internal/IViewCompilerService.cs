namespace TerevintoSoftware.StaticSiteGenerator.Internal;

internal interface IViewCompilerService
{
    Task<IEnumerable<ViewGenerationResult>> CompileViews(IEnumerable<string> viewsToRender);
}

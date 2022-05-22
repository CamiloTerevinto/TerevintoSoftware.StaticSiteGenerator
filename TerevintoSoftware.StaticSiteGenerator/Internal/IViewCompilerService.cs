namespace TerevintoSoftware.StaticSiteGenerator.Internal;

internal interface IViewCompilerService
{
    IAsyncEnumerable<(string, string)> GenerateHtml(IEnumerable<string> viewsToRender);
}

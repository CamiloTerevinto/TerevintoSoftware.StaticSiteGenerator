namespace TerevintoSoftware.StaticSiteGenerator.Configuration;

internal class SiteAssemblyInformation
{
    internal IReadOnlyCollection<string> ViewsFound { get; }
    internal IReadOnlyCollection<string> ControllersFound { get; }

    internal SiteAssemblyInformation(IReadOnlyCollection<string> controllersFound, IReadOnlyCollection<string> viewsFound)
    {
        ViewsFound = viewsFound ?? throw new ArgumentNullException(nameof(viewsFound));
        ControllersFound = controllersFound ?? throw new ArgumentNullException(nameof(controllersFound));
    }
}

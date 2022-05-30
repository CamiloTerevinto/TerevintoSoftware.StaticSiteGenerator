namespace TerevintoSoftware.StaticSiteGenerator.Configuration;

internal class SiteAssemblyInformation
{
    public IReadOnlyCollection<string> ViewsFound { get; set; }
    public IReadOnlyCollection<string> ControllersFound { get; set; }

    public SiteAssemblyInformation(IReadOnlyCollection<string> viewsFound, IReadOnlyCollection<string> controllersFound)
    {
        ViewsFound = viewsFound;
        ControllersFound = controllersFound;
    }
}

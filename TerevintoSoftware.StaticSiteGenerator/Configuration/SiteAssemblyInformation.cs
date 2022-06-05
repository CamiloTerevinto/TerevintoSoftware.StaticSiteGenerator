namespace TerevintoSoftware.StaticSiteGenerator.Configuration;

/// <summary>
/// Contains the types found in the assembly to use for generation.
/// </summary>
internal class SiteAssemblyInformation
{
    /// <summary>
    /// The views that were found in the assembly and can be generated.
    /// </summary>
    internal IReadOnlyCollection<CultureBasedView> Views { get; }

    /// <summary>
    /// The controllers that were found in the assembly.
    /// </summary>
    internal IReadOnlyCollection<string> Controllers { get; }

    internal SiteAssemblyInformation(IReadOnlyCollection<string> controllersFound, IReadOnlyCollection<CultureBasedView> viewsFound)
    {
        Views = viewsFound ?? throw new ArgumentNullException(nameof(viewsFound));
        Controllers = controllersFound ?? throw new ArgumentNullException(nameof(controllersFound));
    }
}

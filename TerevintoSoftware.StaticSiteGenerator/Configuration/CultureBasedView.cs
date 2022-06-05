using System.Diagnostics;

namespace TerevintoSoftware.StaticSiteGenerator.Configuration;

/// <summary>
/// Represents a View to be generated, once per each Culture.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
internal class CultureBasedView
{
    private string DebuggerDisplay => $"{ViewName}. Cultures: {Cultures.Count}";

    /// <summary>
    /// The path-based name of the view. For example: Home/Index.
    /// </summary>
    public string ViewName { get; }

    /// <summary>
    /// The list of cultures to use to generate the view. For example: en-GB, es, fr-FR.
    /// </summary>
    public IReadOnlyCollection<string> Cultures { get; }
    
    public CultureBasedView(string viewName, string defaultCulture)
    {
        ViewName = viewName;
        Cultures = new[] { defaultCulture };
    }

    public CultureBasedView(string viewName, IReadOnlyCollection<string> cultures)
    {
        ViewName = viewName;
        Cultures = cultures;
    }
}

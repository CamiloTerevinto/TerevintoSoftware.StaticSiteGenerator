using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.StaticSiteGenerator.Configuration;

/// <summary>
/// Represents a View to be generated, once per each Culture.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
internal class CultureBasedView(string viewName, IReadOnlyCollection<string> cultures)
{
    [ExcludeFromCodeCoverage]
    private string DebuggerDisplay => $"{ViewName}. Cultures: {Cultures.Count}";

    /// <summary>
    /// The path-based name of the view. For example: Home/Index.
    /// </summary>
    public string ViewName { get; } = viewName;

    /// <summary>
    /// The list of cultures to use to generate the view. For example: en-GB, es, fr-FR.
    /// </summary>
    public IReadOnlyCollection<string> Cultures { get; } = cultures;
}

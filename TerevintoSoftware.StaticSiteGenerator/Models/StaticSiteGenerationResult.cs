namespace TerevintoSoftware.StaticSiteGenerator.Models;

/// <summary>
/// Contains the result of a static site generation.
/// </summary>
public class StaticSiteGenerationResult(IReadOnlyCollection<ViewResult> views, bool success)
{
    /// <summary>
    /// The list of views that were compiled into HTML.
    /// </summary>
    public IReadOnlyCollection<ViewResult> ViewsResults { get; } = views;

    /// <summary>
    /// Returns false if at least one view generation failed.
    /// </summary>
    public bool Success { get; } = success;
}

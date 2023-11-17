namespace TerevintoSoftware.StaticSiteGenerator.Models;

/// <summary>
/// Contains the result of a static site generation.
/// </summary>
public class StaticSiteGenerationResult
{
    /// <summary>
    /// The list of views that were compiled into HTML.
    /// </summary>
    public IReadOnlyCollection<ViewResult> ViewsResults { get; }

    /// <summary>
    /// Returns false if at least one view generation failed.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticSiteGenerationResult"/> class.
    /// </summary>
    public StaticSiteGenerationResult(IReadOnlyCollection<ViewResult> views, bool success)
    {
        ViewsResults = views;
        Success = success;
    }
}

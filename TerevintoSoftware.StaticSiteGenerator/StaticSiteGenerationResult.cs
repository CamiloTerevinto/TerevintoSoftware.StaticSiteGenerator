namespace TerevintoSoftware.StaticSiteGenerator;

/// <summary>
/// Contains the result of a static site generation.
/// </summary>
public class StaticSiteGenerationResult
{
    /// <summary>
    /// Gets the list of views that were compiled into HTML.
    /// </summary>
    public IReadOnlyCollection<string> ViewsCompiled { get; }

    /// <summary>
    /// Gets the list of non-fatal errors that occurred during the generation.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticSiteGenerationResult"/> class.
    /// </summary>
    public StaticSiteGenerationResult(IReadOnlyCollection<string> views, IReadOnlyCollection<string> errors)
    {
        ViewsCompiled = views;
        Errors = errors;
    }
}

using System.Diagnostics;

namespace TerevintoSoftware.StaticSiteGenerator.Internal;

[DebuggerDisplay("{DebuggerDisplay}")]
internal class ViewGenerationResult
{
    public string OriginalViewName { get; set; }
    public GeneratedView? GeneratedView { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Failed { get; set; }

    public ViewGenerationResult(string originalName, GeneratedView generatedView)
    {
        OriginalViewName = originalName;
        GeneratedView = generatedView;
        Failed = false;
    }

    public ViewGenerationResult(string originalName, string errorMessage)
    {
        OriginalViewName = originalName;
        ErrorMessage = errorMessage;
        Failed = true;
    }

    private string DebuggerDisplay => $"{OriginalViewName} => {(Failed ? "failed" : "generated")}";
}

internal class GeneratedView
{
    public string GeneratedName { get; set; }
    public string GeneratedHtml { get; set; }

    public GeneratedView(string generatedName, string generatedHtml)
    {
        GeneratedName = generatedName;
        GeneratedHtml = generatedHtml;
    }
}

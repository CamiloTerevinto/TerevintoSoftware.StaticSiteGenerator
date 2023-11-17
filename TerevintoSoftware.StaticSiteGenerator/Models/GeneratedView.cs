using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.StaticSiteGenerator.Models;

[DebuggerDisplay("{DebuggerDisplay}")]
internal class ViewGenerationResult
{
    public string OriginalViewName { get; }
    public GeneratedView? GeneratedView { get; set; }
    public string? ErrorMessage { get; }
    public bool Failed { get; }
    public string Culture { get; }

    public ViewGenerationResult(string originalName, string culture, GeneratedView generatedView)
    {
        OriginalViewName = originalName;
        GeneratedView = generatedView;
        Failed = false;
        Culture = culture;
    }

    public ViewGenerationResult(string originalName, string culture, string errorMessage)
    {
        OriginalViewName = originalName;
        ErrorMessage = errorMessage;
        Failed = true;
        Culture = culture;
    }

    [ExcludeFromCodeCoverage]
    private string DebuggerDisplay => $"{OriginalViewName} => {(Failed ? "failed" : "generated")}";
}

internal record GeneratedView(string GeneratedName, string GeneratedHtml);

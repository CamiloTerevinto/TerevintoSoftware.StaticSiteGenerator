using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.StaticSiteGenerator.Models;

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

    [ExcludeFromCodeCoverage]
    private string DebuggerDisplay => $"{OriginalViewName} => {(Failed ? "failed" : "generated")}";
}

internal record GeneratedView(string GeneratedName, string GeneratedHtml, string? Culture);

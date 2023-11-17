namespace TerevintoSoftware.StaticSiteGenerator.Models;

public record ViewResult(string ViewName, List<ViewCultureResult> Results);

public record ViewCultureResult(string Culture, string Error, string GeneratedView);
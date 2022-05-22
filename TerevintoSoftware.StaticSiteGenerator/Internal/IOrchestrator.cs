namespace TerevintoSoftware.StaticSiteGenerator.Internal;

internal interface IOrchestrator
{
    Task<StaticSiteGenerationResult> BuildStaticFilesAsync();
}
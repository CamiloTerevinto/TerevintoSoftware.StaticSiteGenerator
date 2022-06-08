using TerevintoSoftware.StaticSiteGenerator.Models;

namespace TerevintoSoftware.StaticSiteGenerator.Services;

internal interface IOrchestrator
{
    Task<StaticSiteGenerationResult> BuildStaticFilesAsync();
}
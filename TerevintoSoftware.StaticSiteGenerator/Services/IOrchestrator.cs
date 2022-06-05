using TerevintoSoftware.StaticSiteGenerator.Models;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal interface IOrchestrator
{
    Task<StaticSiteGenerationResult> BuildStaticFilesAsync();
}
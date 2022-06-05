using Microsoft.Extensions.DependencyInjection;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;
using TerevintoSoftware.StaticSiteGenerator.Models;

namespace TerevintoSoftware.StaticSiteGenerator;

public static class StaticSiteBuilder
{
    public static async Task<StaticSiteGenerationResult> GenerateStaticSite(StaticSiteGenerationOptions staticSiteOptions)
    {
        if (staticSiteOptions == null)
        {
            throw new ArgumentNullException(nameof(staticSiteOptions));
        }

        await using var app = new Startup()
            .ConfigureServices(staticSiteOptions)
            .Build();

        var orchestrator = app.Services.GetRequiredService<IOrchestrator>();

        return await orchestrator.BuildStaticFilesAsync();
    }
}

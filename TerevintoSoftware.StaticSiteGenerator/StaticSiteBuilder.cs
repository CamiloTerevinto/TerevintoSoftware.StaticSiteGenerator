using Microsoft.Extensions.DependencyInjection;
using TerevintoSoftware.StaticSiteGenerator.Models;
using TerevintoSoftware.StaticSiteGenerator.Services;

namespace TerevintoSoftware.StaticSiteGenerator;

public static class StaticSiteBuilder
{
    /// <summary>
    /// Generates a static site using the options provided.
    /// </summary>
    /// <param name="staticSiteOptions">The options to use to generate the static site.</param>
    /// <param name="writeOutputLogs">Whether to write output logs after the generation finishes.</param>
    /// <returns>The result of the static site generation.</returns>
    /// <exception cref="ArgumentNullException">When staticSiteOptions is null.</exception>
    public static async Task<StaticSiteGenerationResult> GenerateStaticSite(StaticSiteGenerationOptions staticSiteOptions, bool writeOutputLogs)
    {
        if (staticSiteOptions == null)
        {
            throw new ArgumentNullException(nameof(staticSiteOptions));
        }

        await using var app = new Startup(staticSiteOptions)
            .ConfigureServices()
            .Build();

        var orchestrator = app.Services.GetRequiredService<IOrchestrator>();

        var result = await orchestrator.BuildStaticFilesAsync();

        if (writeOutputLogs)
        {
            orchestrator.LogResults(result);
        }

        return result;
    }
}

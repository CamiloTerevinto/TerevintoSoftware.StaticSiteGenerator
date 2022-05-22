namespace TerevintoSoftware.StaticSiteGenerator;

public static class StaticSiteBuilder
{
    public static async Task<StaticSiteGenerationResult> GenerateStaticSite(StaticSiteGenerationOptions staticSiteOptions)
    {
        if (staticSiteOptions == null)
        {
            throw new ArgumentNullException(nameof(staticSiteOptions));
        }

        var orchestrator = new Startup()
            .ConfigureServices(staticSiteOptions)
            .BuildServices();

        return await orchestrator.BuildStaticFilesAsync();
    }
}

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using TerevintoSoftware.StaticSiteGenerator.Internal;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;

namespace TerevintoSoftware.StaticSiteGenerator;

internal class Startup
{
    private readonly ServiceCollection _services;

    internal Startup()
    {
        _services = new ServiceCollection();
    }

    internal Startup ConfigureServices(StaticSiteGenerationOptions staticSiteOptions)
    {
        _services.AddMvc();

        _services.Configure<RazorViewEngineOptions>(opt =>
        {
            foreach (var extraViewsFormat in staticSiteOptions.ViewsLocationFormats)
            {
                opt.ViewLocationFormats.Add(extraViewsFormat);
            }
        });

        _services
            .AddSingleton(staticSiteOptions)
            .AddSingleton<IViewCompilerService, ViewCompilerService>()
            .AddSingleton<IStaticAssetsFactory, StaticAssetsFactory>()
            .AddSingleton<IOrchestrator, Orchestrator>();
        
        return this;
    }

    internal IOrchestrator BuildServices()
    {
        var app = _services.BuildServiceProvider();
        return app.GetRequiredService<IOrchestrator>();
    }
}

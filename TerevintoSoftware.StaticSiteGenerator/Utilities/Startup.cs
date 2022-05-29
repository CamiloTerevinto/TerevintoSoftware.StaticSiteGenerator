using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;
using TerevintoSoftware.StaticSiteGenerator.Internal;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;

namespace TerevintoSoftware.StaticSiteGenerator;

internal class Startup
{
    private readonly WebApplicationBuilder _builder;

    internal Startup()
    {
        _builder = WebApplication.CreateBuilder();
    }

    internal Startup ConfigureServices(StaticSiteGenerationOptions staticSiteOptions)
    {
        try
        {
            var listener = new DiagnosticListener("StaticSiteGenerator");
            var assembly = Assembly.LoadFrom(staticSiteOptions.AssemblyPath);

            _builder.Services.AddSingleton<DiagnosticSource>(listener);
            _builder.Services.AddSingleton(listener);
            _builder.Services.AddLogging(c => { c.AddConsole(); });

            _builder.Services
                .AddControllersWithViews()
                .AddApplicationPart(assembly)
                .AddRazorRuntimeCompilation();

            var endpointProvider = new EndpointProvider(staticSiteOptions);
            endpointProvider.Inject(_builder.Services);

            _builder.Services
                .AddSingleton<IEndpointProvider>(endpointProvider)
                .AddSingleton(staticSiteOptions)
                .AddSingleton<IViewCompilerService, ViewCompilerService>()
                .AddSingleton<IOrchestrator, Orchestrator>();

            return this;
        }
        catch (Exception ex)
        {
            throw new Exception("Error while configuring services", ex);
        }
    }

    internal WebApplication Build()
    {
        return _builder.Build();
    }
}

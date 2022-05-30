using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;
using TerevintoSoftware.StaticSiteGenerator.Internal;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;
using TerevintoSoftware.StaticSiteGenerator.Configuration;

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
            _builder.Services.AddLogging(c =>
            {
                c.AddConsole();
                
                if (staticSiteOptions.Verbose)
                {
                    c.SetMinimumLevel(LogLevel.Debug);
                }
            });


            _builder.Services
                .AddControllersWithViews()
                .AddApplicationPart(assembly)
                .AddRazorRuntimeCompilation();

            var endpointProvider = new EndpointProvider(staticSiteOptions);
            endpointProvider.Inject(_builder.Services);

            var siteAssemblyInformation = new SiteAssemblyInformation(FindViewsInAssembly(assembly), FindControllersInAssembly(assembly));
            
            _builder.Services
                .AddSingleton<IEndpointProvider>(endpointProvider)
                .AddSingleton(staticSiteOptions)
                .AddSingleton(siteAssemblyInformation)
                .AddSingleton<IUrlFormatter, UrlFormatter>()
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

    private static IReadOnlyCollection<string> FindViewsInAssembly(Assembly assembly)
    {
        var nonModelViewBaseType = typeof(RazorPage<object>);

        return assembly.GetCustomAttributes<RazorCompiledItemAttribute>()
            .Where(x => nonModelViewBaseType.IsAssignableFrom(x.Type) &&
                        !x.Identifier.Contains("_Layout") &&
                        !x.Identifier.Contains("_ViewStart") &&
                        !x.Identifier.Contains("_ViewImports"))
            .Select(x =>
            {
                // remove /views/ from x.Identifier
                var identifier = x.Identifier.Substring(7);

                // remove .cshtml from identifier
                var index = identifier.LastIndexOf(".cshtml", StringComparison.Ordinal);
                if (index > 0)
                {
                    identifier = identifier.Substring(0, index);
                }

                return identifier;
            }).ToArray();
    }

    private static IReadOnlyCollection<string> FindControllersInAssembly(Assembly assembly)
    {
        var controllerBaseType = typeof(Controller);

        return assembly.GetExportedTypes()
            .Where(x => controllerBaseType.IsAssignableFrom(x))
            .Select(x =>
            {
                if (x.Name.EndsWith("Controller"))
                {
                    return x.Name.Substring(0, x.Name.Length - 10);
                }

                return x.Name;
            }).ToArray();
    }
}

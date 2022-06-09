using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;
using TerevintoSoftware.StaticSiteGenerator.Services;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator;

[ExcludeFromCodeCoverage] // There is no obvious way to unit test this
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

            var exportedTypes = assembly.GetExportedTypes();
            var customAttributes = assembly.GetCustomAttributes();
            var siteAssemblyInformation = SiteAssemblyInformationFactory.GetAssemblyInformation(exportedTypes, customAttributes, staticSiteOptions.DefaultCulture);
            var cultures = CultureHelpers.GetUniqueCultures(staticSiteOptions, siteAssemblyInformation);

            _builder.Services
                .AddControllersWithViews()
                .AddApplicationPart(assembly)
                .AddRazorRuntimeCompilation()
                .AddViewLocalization();

            if (cultures.Count > 1)
            {
                _builder.Services.AddRequestLocalization(opt =>
                {
                    opt.SupportedCultures = cultures;
                    opt.SupportedUICultures = cultures;
                    opt.DefaultRequestCulture = new RequestCulture(staticSiteOptions.DefaultCulture);
                });
            }

            var endpointProvider = new EndpointProvider(staticSiteOptions);
            endpointProvider.Inject(_builder.Services);

            _builder.Services
                .AddSingleton<IEndpointProvider>(endpointProvider)
                .AddSingleton(staticSiteOptions)
                .AddSingleton(siteAssemblyInformation)
                .AddSingleton<IHtmlFormatter, HtmlFormatter>()
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
}

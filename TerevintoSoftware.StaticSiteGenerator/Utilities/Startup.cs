using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Services;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator;

[ExcludeFromCodeCoverage] // There is no obvious way to unit test this
internal class Startup
{
    private readonly WebApplicationBuilder _builder;
    private readonly StaticSiteGenerationOptions _staticSiteOptions;

    internal Startup(StaticSiteGenerationOptions staticSiteOptions)
    {
        _builder = WebApplication.CreateBuilder(new WebApplicationOptions { ContentRootPath = staticSiteOptions.ProjectPath, WebRootPath = staticSiteOptions.ProjectPath });
        _staticSiteOptions = staticSiteOptions;
    }

    internal Startup ConfigureServices()
    {
        try
        {
            SetUpLogging(_staticSiteOptions.Verbose);
            var assembly = Assembly.LoadFrom(_staticSiteOptions.AssemblyPath);

            var exportedTypes = assembly.GetExportedTypes();
            var customAttributes = assembly.GetCustomAttributes();
            var siteAssemblyInformation = SiteAssemblyInformationFactory
                .BuildAssemblyInformation(exportedTypes, customAttributes, _staticSiteOptions.DefaultCulture);

            _builder.Services
                .AddControllersWithViews()
                .AddApplicationPart(assembly)
                .AddRazorRuntimeCompilation()
                .AddViewLocalization();

            SetUpGlobalization(_staticSiteOptions, siteAssemblyInformation);

            var endpointProvider = new EndpointProvider(_staticSiteOptions);
            endpointProvider.Inject(_builder.Services);

            _builder.Services
                .AddSingleton<IEndpointProvider>(endpointProvider)
                .AddSingleton(_staticSiteOptions)
                .AddSingleton(siteAssemblyInformation)
                .AddSingleton<IHtmlFormatter, HtmlFormatter>()
                .AddSingleton<IUrlFormatter, UrlFormatter>()
                .AddSingleton<IViewCompilerService, ViewCompilerService>()
                .AddSingleton<IViewRenderService, ViewRenderService>()
                .AddSingleton<IActionContextFactory, ActionContextFactory>()
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

    private void SetUpLogging(bool enableVerboseLogs)
    {
        var listener = new DiagnosticListener("StaticSiteGenerator");
        _builder.Services.AddSingleton<DiagnosticSource>(listener);
        _builder.Services.AddSingleton(listener);
        _builder.Services.AddLogging(c =>
        {
            c.AddConsole();

            if (enableVerboseLogs)
            {
                c.SetMinimumLevel(LogLevel.Debug);
            }

            c.AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Warning);
        });
    }

    private void SetUpGlobalization(StaticSiteGenerationOptions staticSiteOptions, SiteAssemblyInformation siteAssemblyInformation)
    {
        var cultures = CultureHelpers.GetUniqueCultures(staticSiteOptions, siteAssemblyInformation);
        if (cultures.Count > 1)
        {
            _builder.Services.AddRequestLocalization(opt =>
            {
                opt.SupportedCultures = cultures;
                opt.SupportedUICultures = cultures;
                opt.DefaultRequestCulture = new RequestCulture(staticSiteOptions.DefaultCulture);
            });
        }
    }
}

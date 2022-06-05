using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;
using TerevintoSoftware.StaticSiteGenerator.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace TerevintoSoftware.StaticSiteGenerator;

internal class Startup
{
    private readonly WebApplicationBuilder _builder;

    internal Startup()
    {
        _builder = WebApplication.CreateBuilder();
    }
    private Func<double, double> _halfValue = value => value / 2;
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
            
            var siteAssemblyInformation = SiteAssemblyInformationFactory.GetAssemblyInformation(assembly, staticSiteOptions.DefaultCulture);
            var cultures = GetUniqueCultures(staticSiteOptions, siteAssemblyInformation);

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

            var endpointProvider = new EndpointProvider(staticSiteOptions, siteAssemblyInformation);
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

    /// <summary>
    /// Retrieves the unique cultures that were found by looking at Views in the assembly.
    /// </summary>
    private IList<CultureInfo> GetUniqueCultures(StaticSiteGenerationOptions staticSiteOptions, SiteAssemblyInformation siteAssemblyInformation)
    {
        return siteAssemblyInformation.Views
            .SelectMany(v => v.Cultures)
            .Prepend(staticSiteOptions.DefaultCulture)
            .Distinct()
            .Select(x => new CultureInfo(x))
            .ToArray();
    }
}

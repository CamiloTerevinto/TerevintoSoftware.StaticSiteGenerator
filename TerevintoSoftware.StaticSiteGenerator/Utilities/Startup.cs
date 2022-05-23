using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Reflection;
using TerevintoSoftware.StaticSiteGenerator.Internal;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;

namespace TerevintoSoftware.StaticSiteGenerator;

internal class TestWebHostEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; }
    public IFileProvider WebRootFileProvider { get; set; }
    public string ApplicationName { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
    public string ContentRootPath { get; set; }
    public string EnvironmentName { get; set; }

    public TestWebHostEnvironment(string webRootPath, string contentRootPath, string assemblyName)
    {
        WebRootPath = webRootPath;
        WebRootFileProvider = new PhysicalFileProvider(WebRootPath);
        ApplicationName = assemblyName;
        ContentRootPath = contentRootPath;
        ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
        EnvironmentName = "Development";
    }
}

internal class Startup
{
    private readonly ServiceCollection _services;

    internal Startup()
    {
        _services = new ServiceCollection();
    }

    internal Startup ConfigureServices(StaticSiteGenerationOptions staticSiteOptions)
    {
        try
        {
            var assemblyName = Path.GetFileName(staticSiteOptions.AssemblyPath);
            var copiedPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyName);
            File.Copy(staticSiteOptions.AssemblyPath, copiedPath, true);

            var listener = new DiagnosticListener("StaticSiteGenerator");
            var assembly = Assembly.LoadFrom(copiedPath);

            _services.AddSingleton<DiagnosticSource>(listener);
            _services.AddSingleton(listener);
            _services.AddLogging();

            _services.AddSingleton<IWebHostEnvironment>(
                new TestWebHostEnvironment(staticSiteOptions.ProjectPath, Path.Combine(staticSiteOptions.ProjectPath, "wwwroot"), assembly.GetName().Name!));

            _services
                .AddMvc()
                .AddApplicationPart(assembly)
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(assembly));
                });

            _services
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

    internal IOrchestrator BuildServices()
    {
        var app = _services.BuildServiceProvider();
        return app.GetRequiredService<IOrchestrator>();
    }
}

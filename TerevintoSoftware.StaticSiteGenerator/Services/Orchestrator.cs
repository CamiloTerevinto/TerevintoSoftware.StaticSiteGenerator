using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Models;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Services;

internal class Orchestrator : IOrchestrator
{
    private readonly IViewCompilerService _viewCompilerService;
    private readonly StaticSiteGenerationOptions _staticSiteOptions;
    private readonly SiteAssemblyInformation _siteAssemblyInformation;
    private readonly ILogger<Orchestrator> _logger;

    public Orchestrator(IViewCompilerService viewCompilerService, StaticSiteGenerationOptions staticSiteOptions, 
        SiteAssemblyInformation siteAssemblyInformation, ILogger<Orchestrator> logger)
    {
        _viewCompilerService = viewCompilerService;
        _staticSiteOptions = staticSiteOptions;
        _siteAssemblyInformation = siteAssemblyInformation;
        _logger = logger;
    }

    public async Task<StaticSiteGenerationResult> BuildStaticFilesAsync()
    {
        var destination = _staticSiteOptions.OutputPath;

        if (Directory.Exists(destination) && _staticSiteOptions.ClearExistingOutput)
        {
            _logger.LogInformation("Deleting old output files");
            Directory.Delete(destination, true);
        }

        _logger.LogInformation("Processing started...");

        Directory.CreateDirectory(destination);

        var viewsTask = GenerateViewsAsync();
        var assetsTask = Task.Run(CopyStaticAssets);

        await Task.WhenAll(viewsTask, assetsTask);

        var (errors, views) = viewsTask.Result;

        _logger.LogInformation("Processing completed.");

        return new StaticSiteGenerationResult(views, errors);
    }

    public void LogResults(StaticSiteGenerationResult result)
    {
        _logger.LogInformation("Generated {FilesCount} files:", result.ViewsCompiled.Count);

        foreach (var view in result.ViewsCompiled)
        {
            _logger.LogInformation("{GeneratedView}", view);
        }

        if (result.Errors.Count > 0)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("{Error}", error);
            }

            if (!_staticSiteOptions.Verbose)
            {
                _logger.LogInformation("Use --verbose to see detailed errors.");
            }
        }
    }

    private async Task<(List<string>, List<string>)> GenerateViewsAsync()
    {
        var views = new List<string>();
        var errors = new List<string>();

        _logger.LogInformation("Found {ViewCount} views to compile in {ControllerCount} controllers",
            _siteAssemblyInformation.Views.Count, _siteAssemblyInformation.Controllers.Count);
        var viewsToGenerate = _siteAssemblyInformation.Views;

        var viewGenerationResults = await _viewCompilerService.CompileViews(viewsToGenerate);
        var baseControllerPath = $"{_staticSiteOptions.BaseController.ToLower()}/";

        foreach (var generationResult in viewGenerationResults)
        {
            if (generationResult.Failed)
            {
                errors.Add($"View {generationResult.OriginalViewName} => {generationResult.ErrorMessage!}");
                continue;
            }

            var staticViewPath = GetNewViewPath(baseControllerPath, generationResult);

            if (!Directory.Exists(Path.GetDirectoryName(staticViewPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(staticViewPath)!);
            }

            var view = generationResult.GeneratedView!;

            File.WriteAllText(staticViewPath, view.GeneratedHtml);

            // If this is the main/default view and also the default language,
            // we copy the file again as just /index.html, so it can serve as an entry point
            // for hosting providers that asks for an index.html at the root
            if (_staticSiteOptions.UseLocalization &&
                generationResult.OriginalViewName.ToLower() == $"{_staticSiteOptions.BaseController.ToLower()}/index" &&
                view.Culture == _staticSiteOptions.DefaultCulture)
            {
                File.WriteAllText(Path.Combine(_staticSiteOptions.OutputPath, Path.GetFileName(staticViewPath)), view.GeneratedHtml);
            }

            views.Add($"View {generationResult.OriginalViewName} => {view.GeneratedName}");
        }

        return (errors, views);
    }

    private string GetNewViewPath(string baseControllerPath, ViewGenerationResult generationResult)
    {
        var generatedName = generationResult.GeneratedView!.GeneratedName.ToCasing(_staticSiteOptions.RouteCasing);

        if (generatedName.StartsWith(baseControllerPath))
        {
            generatedName = generatedName[baseControllerPath.Length..];
        }

        if (generationResult.GeneratedView.Culture != null && _staticSiteOptions.UseLocalization)
        {
            generatedName = $"{generationResult.GeneratedView.Culture}/{generatedName}";
        }

        generationResult.GeneratedView = generationResult.GeneratedView with { GeneratedName = generatedName };

        return Path.Combine(_staticSiteOptions.OutputPath, generatedName);
    }

    private void CopyStaticAssets()
    {
        var origin = Path.Combine(_staticSiteOptions.ProjectPath, "wwwroot");
        var destination = _staticSiteOptions.OutputPath;

        var directories = Directory.EnumerateDirectories(origin, "*.*", SearchOption.AllDirectories);

        foreach (var directory in directories)
        {
            Directory.CreateDirectory(directory.Replace(origin, destination));
        }

        var files = Directory.EnumerateFiles(origin, "*.*", SearchOption.AllDirectories);

        Parallel.ForEach(files, filePath =>
        {
            File.Copy(filePath, filePath.Replace(origin, destination));
        });
    }
}

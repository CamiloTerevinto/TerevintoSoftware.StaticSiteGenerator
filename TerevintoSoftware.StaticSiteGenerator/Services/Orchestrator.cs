using Spectre.Console;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Models;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Services;

internal interface IOrchestrator
{
    Task<StaticSiteGenerationResult> BuildStaticFilesAsync();
    void LogResults(StaticSiteGenerationResult result);
}

internal class Orchestrator(IViewCompilerService viewCompilerService, StaticSiteGenerationOptions staticSiteOptions,
    SiteAssemblyInformation siteAssemblyInformation) : IOrchestrator
{
    private readonly IViewCompilerService _viewCompilerService = viewCompilerService;
    private readonly StaticSiteGenerationOptions _staticSiteOptions = staticSiteOptions;
    private readonly SiteAssemblyInformation _siteAssemblyInformation = siteAssemblyInformation;

    public async Task<StaticSiteGenerationResult> BuildStaticFilesAsync()
    {
        AnsiConsole.MarkupLine("Processing started...");
        
        var destination = _staticSiteOptions.OutputPath;

        if (Directory.Exists(destination) && _staticSiteOptions.ClearExistingOutput)
        {
            AnsiConsole.MarkupLine("[bold]Deleting old output files[/]");
            Directory.Delete(destination, true);
        }

        Directory.CreateDirectory(destination);

        var viewsTask = GenerateViewsAsync();
        var assetsTask = Task.Run(CopyStaticAssets);

        await Task.WhenAll(viewsTask, assetsTask);

        var (viewResults, success) = viewsTask.Result;

        AnsiConsole.MarkupLine("Processing completed.");

        return new StaticSiteGenerationResult(viewResults, success);
    }

    public void LogResults(StaticSiteGenerationResult result)
    {
        foreach (var view in result.ViewsResults)
        {
            AnsiConsole.MarkupLine("{0} results: {1} generated, {2} failed to generate.", view.ViewName, 
                view.Results.Count(x => x.GeneratedView != null), view.Results.Count(x => x.Error != null));
        }
    }

    private async Task<(List<ViewResult>, bool)> GenerateViewsAsync()
    {
        var result = new List<ViewResult>();
        var success = true;

        AnsiConsole.MarkupLine("Found {0} views to compile in {1} controllers", _siteAssemblyInformation.Views.Count, _siteAssemblyInformation.Controllers.Count);
        var viewsToGenerate = _siteAssemblyInformation.Views;

        var viewGenerationResults = await _viewCompilerService.CompileViews(viewsToGenerate);
        var baseControllerPath = $"{_staticSiteOptions.BaseController.ToLower()}{Path.DirectorySeparatorChar}";

        var tempResult = new Dictionary<string, List<ViewCultureResult>>();

        foreach (var generationResult in viewGenerationResults)
        {
            var nonCulturedViewName = generationResult.OriginalViewName;

            if (nonCulturedViewName.Contains('.'))
            {
                nonCulturedViewName = nonCulturedViewName[..nonCulturedViewName.IndexOf('.')];
            }

            if (!tempResult.TryGetValue(nonCulturedViewName, out var nonCulturedView))
            {
                nonCulturedView = new();
                tempResult.Add(nonCulturedViewName, nonCulturedView);
            }

            if (generationResult.Failed)
            {
                nonCulturedView.Add(new(generationResult.Culture, generationResult.ErrorMessage!, null!));
                success = false;
                continue;
            }

            var staticViewPath = GetNewViewPath(baseControllerPath, generationResult);
            Directory.CreateDirectory(Path.GetDirectoryName(staticViewPath)!);

            var view = generationResult.GeneratedView!;

            File.WriteAllText(staticViewPath, view.GeneratedHtml);

            // If this is the main/default view and also the default language,
            // we copy the file again as just /index.html, so it can serve as an entry point
            // for hosting providers that ask for an index.html at the root
            if (_staticSiteOptions.UseLocalization &&
                generationResult.OriginalViewName.Equals($"{_staticSiteOptions.BaseController.ToLower()}/index", StringComparison.CurrentCultureIgnoreCase) &&
                generationResult.Culture == _staticSiteOptions.DefaultCulture)
            {
                File.WriteAllText(Path.Combine(_staticSiteOptions.OutputPath, Path.GetFileName(staticViewPath)), view.GeneratedHtml);
            }

            nonCulturedView.Add(new(generationResult.Culture, null!, staticViewPath));
        }

        var finalResult = tempResult.Select(x => new ViewResult(x.Key, x.Value)).ToList();

        return (finalResult, success);
    }

    private string GetNewViewPath(string baseControllerPath, ViewGenerationResult generationResult)
    {
        var generatedName = generationResult.GeneratedView!.GeneratedName
            .ToCasing(_staticSiteOptions.RouteCasing)
            .Replace('/', Path.DirectorySeparatorChar);

        if (generatedName.StartsWith(baseControllerPath))
        {
            generatedName = generatedName[baseControllerPath.Length..];
        }

        if (generationResult.Culture != null && _staticSiteOptions.UseLocalization)
        {
            generatedName = generationResult.Culture + Path.DirectorySeparatorChar + generatedName;
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

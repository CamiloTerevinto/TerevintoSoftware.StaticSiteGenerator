using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal class Orchestrator : IOrchestrator
{
    private readonly IViewCompilerService _viewCompilerService;
    private readonly StaticSiteGenerationOptions _staticSiteOptions;
    private readonly SiteAssemblyInformation _siteAssemblyInformation;

    public Orchestrator(IViewCompilerService viewCompilerService,
        StaticSiteGenerationOptions staticSiteOptions, SiteAssemblyInformation siteAssemblyInformation)
    {
        _viewCompilerService = viewCompilerService;
        _staticSiteOptions = staticSiteOptions;
        _siteAssemblyInformation = siteAssemblyInformation;
    }

    public async Task<StaticSiteGenerationResult> BuildStaticFilesAsync()
    {
        var viewsTask = GenerateViewsAsync();
        var assetsTask = Task.Run(CopyStaticAssets);

        await Task.WhenAll(viewsTask, assetsTask);

        var (errors, views) = viewsTask.Result;

        return new StaticSiteGenerationResult(views, errors);
    }

    private async Task<(List<string>, List<string>)> GenerateViewsAsync()
    {
        var views = new List<string>();
        var errors = new List<string>();

        var viewsToGenerate = _siteAssemblyInformation.ViewsFound;

        var viewGenerationResults = await _viewCompilerService.CompileViews(viewsToGenerate);
        var baseControllerPath = $"{_staticSiteOptions.BaseController.ToLower()}/";

        foreach (var generationResult in viewGenerationResults)
        {
            if (generationResult.Failed)
            {
                errors.Add($"View {generationResult.OriginalViewName} => {generationResult.ErrorMessage!}");
                continue;
            }

            var view = generationResult.GeneratedView!;
            view.GeneratedName = view.GeneratedName.ToLower();

            if (view.GeneratedName.StartsWith(baseControllerPath))
            {
                view.GeneratedName = view.GeneratedName[baseControllerPath.Length..];
            }

            var staticViewPath = Path.Combine(_staticSiteOptions.OutputPath, view.GeneratedName);

            if (!Directory.Exists(Path.GetDirectoryName(staticViewPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(staticViewPath)!);
            }

            File.WriteAllText(staticViewPath, view.GeneratedHtml);

            views.Add($"View {generationResult.OriginalViewName} => {view.GeneratedName}");
        }

        return (errors, views);
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

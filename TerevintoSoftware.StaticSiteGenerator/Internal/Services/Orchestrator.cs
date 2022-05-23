using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Reflection;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal class Orchestrator : IOrchestrator
{
    private readonly IViewCompilerService _viewCompilerService;
    private readonly StaticSiteGenerationOptions _staticSiteOptions;

    public Orchestrator(IViewCompilerService viewCompilerService, StaticSiteGenerationOptions staticSiteOptions)
    {
        _viewCompilerService = viewCompilerService;
        _staticSiteOptions = staticSiteOptions;
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

        var viewsToGenerate = FindViewsInAssembly(Assembly.LoadFrom(_staticSiteOptions.AssemblyPath));
        
        var viewGenerationResults = await _viewCompilerService.CompileViews(viewsToGenerate);
        var baseControllerPath = $"{_staticSiteOptions.BaseController}/";

        foreach (var generationResult in viewGenerationResults)
        {
            if (generationResult.Failed)
            {
                errors.Add($"View {generationResult.OriginalViewName} => {generationResult.ErrorMessage!}");
            }
            else
            {
                var view = generationResult.GeneratedView!;

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
}

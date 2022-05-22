using System.Collections.Concurrent;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal class Orchestrator : IOrchestrator
{
    private readonly IStaticAssetsFactory _staticAssetsFactory;
    private readonly IViewCompilerService _viewCompilerService;
    private readonly StaticSiteGenerationOptions _staticSiteOptions;

    public Orchestrator(IStaticAssetsFactory staticAssetsFactory, IViewCompilerService viewCompilerService, 
        StaticSiteGenerationOptions staticSiteOptions)
    {
        _staticAssetsFactory = staticAssetsFactory;
        _viewCompilerService = viewCompilerService;
        _staticSiteOptions = staticSiteOptions;
    }
    
    public async Task<StaticSiteGenerationResult> BuildStaticFilesAsync()
    {
        var htmlFiles = new ConcurrentBag<string>();

        await foreach (var (viewName, viewHtml) in _viewCompilerService.GenerateHtml(_staticSiteOptions.ViewsToGenerate))
        {
            htmlFiles.Add(viewName);
            
            await _staticAssetsFactory.ProcessReferencesInHtml(viewHtml);
        }

        var assetsResult = _staticAssetsFactory.Build();

        var result = new StaticSiteGenerationResult
        {
            CssFilesCopied = assetsResult.CssFiles,
            ImagesCopied = assetsResult.Images,
            JsFilesCopied = assetsResult.JsFiles,
            VideosCopied = assetsResult.Videos,
            ViewsCompiled = htmlFiles
        };

        return result;
    }
}

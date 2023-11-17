using Spectre.Console;
using Spectre.Console.Cli;

namespace TerevintoSoftware.StaticSiteGenerator.Tool;

internal class GenerateCommand : AsyncCommand<GenerateSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GenerateSettings settings)
    {
        try
        {
            var generationResult = await StaticSiteBuilder.GenerateStaticSite(GetStaticSiteGenerationOptions(settings), writeOutputLogs: false);

            var tree = new Tree("Static Site Generated");

            tree.AddNode($"Output generated at: [bold]{settings.OutputPath}[/]");

            var controllerGrouping = generationResult.ViewsResults
                .Select(x => 
                {
                    var viewNameParts = x.ViewName.Split('/');

                    return new
                    {
                        Controller = viewNameParts[0],
                        ViewName = viewNameParts[1],
                        x.Results
                    };
                })
                .GroupBy(x => x.Controller);

            foreach (var grouping in controllerGrouping)
            {
                var node = tree.AddNode(grouping.Key);

                foreach (var view in grouping)
                {
                    var viewNode = node.AddNode(view.ViewName);

                    foreach (var result in view.Results)
                    {
                        var shortenedPath = result.GeneratedView?.Replace(settings.OutputPath + Path.DirectorySeparatorChar, string.Empty);
                        var resultStatus = result.GeneratedView != null ? $"[green]{shortenedPath}[/]" : $"[red]{result.Error}[/]";
                        viewNode.AddNode($"[bold]{result.Culture}:[/] {resultStatus}");
                    }                    
                }
            }

            AnsiConsole.Write(tree);

            return generationResult.Success ? 0 : 1;
        }
        catch (Exception ex)
        {
            if (settings.Verbose)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShowLinks);
            }
            else
            {
                AnsiConsole.WriteLine($"[red]{ex.Message}[/]");
                AnsiConsole.WriteLine("Use the --verbose option to see more details.");
            }
        }

        return -1;
    }

    private static StaticSiteGenerationOptions GetStaticSiteGenerationOptions(GenerateSettings settings)
    {
        return new StaticSiteGenerationOptions(settings.ProjectPath, settings.OutputPath, settings.AssemblyPath, settings.BaseController,
            settings.DefaultRouteTemplate, settings.RouteCasing, settings.DefaultCulture, settings.UseLocalization,
            settings.ClearExistingOutput, settings.Verbose);
    }
}

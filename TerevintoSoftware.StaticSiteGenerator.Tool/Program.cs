using System.CommandLine;

namespace TerevintoSoftware.StaticSiteGenerator.Tool;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = StaticSiteGenerationOptionsBinder.BuildRootCommand();
        
        return await rootCommand.InvokeAsync(args);
    }
}
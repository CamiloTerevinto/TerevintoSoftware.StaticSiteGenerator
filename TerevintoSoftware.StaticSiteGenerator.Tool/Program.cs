using System.CommandLine;
using TerevintoSoftware.StaticSiteGenerator.Tool;

var rootCommand = StaticSiteGenerationOptionsBinder.BuildRootCommand();

return await rootCommand.InvokeAsync(args);

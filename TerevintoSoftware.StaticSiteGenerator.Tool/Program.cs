using System.CommandLine;
using TerevintoSoftware.StaticSiteGenerator.Tool;

var rootCommand = StaticSiteGenerationOptionsBinder.BuildRootCommand();

args = new[]
{
    "--project",
    "D:\\Source\\CT.Website\\CT.Website",
    "--output",
    "D:\\Source\\CT.Website\\static",
    "--route-casing",
    "KebabCase",
    "--assembly",
    "bin/Release/net6.0/CT.Website.dll"
};

return await rootCommand.InvokeAsync(args);

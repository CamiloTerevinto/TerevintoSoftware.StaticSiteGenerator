using System.CommandLine;

namespace TerevintoSoftware.StaticSiteGenerator.Tool;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var projectPathOption = new Option<string?>(
            "--project-path",
            parseArgument: result =>
            {
                if (result.Tokens.Count != 1)
                {
                    result.ErrorMessage = "Missing project path";
                    return null;
                }

                var projectPath = result.Tokens.Single().Value;

                if (!Directory.Exists(projectPath))
                {
                    result.ErrorMessage = $"Project path '{projectPath}' does not exist";
                    return null;
                }

                return projectPath;
            },
            description: "The path to the project to generate the static site for.");
        projectPathOption.IsRequired = true;
        
        var outputPathOption = new Option<string?>(
            "--output-path",
            parseArgument: result =>
            {
                if (result.Tokens.Count != 1)
                {
                    result.ErrorMessage = "Missing output path";
                    return null;
                }

                var outputPath = result.Tokens.Single().Value;

                if (!Directory.Exists(outputPath))
                {
                    result.ErrorMessage = $"Output path '{outputPath}' does not exist";
                    return null;
                }

                return outputPath;
            },
            description: "The path to the output directory.");
        outputPathOption.IsRequired = true;

        var assemblyPathOption = new Option<string?>(
            "--assembly-path",
            parseArgument: result =>
            {
                if (result.Tokens.Count != 1)
                {
                    result.ErrorMessage = "Missing assembly path";
                    return null;
                }

                var assemblyPath = result.Tokens.Single().Value;

                if (!File.Exists(assemblyPath))
                {
                    result.ErrorMessage = $"The assembly file '{assemblyPath}' does not exist";
                    return null;
                }

                return assemblyPath;
            },
            description: "The path to the assembly to use for the project.");
        assemblyPathOption.IsRequired = true;

        var baseControllerOption = new Option<string>(
            "--base-controller",
            parseArgument: result =>
            {
                if (result.Tokens.Count != 1)
                {
                    return "Home";
                }

                return result.Tokens.Single().Value;
            },
            description: "The base controller to use for the project.");

        var rootCommand = new RootCommand();
        rootCommand.AddOption(projectPathOption);
        rootCommand.AddOption(outputPathOption);
        rootCommand.AddOption(assemblyPathOption);
        rootCommand.AddOption(baseControllerOption);

        rootCommand.SetHandler<string, string, string, string>(
            async (projectPath, outputPath, assemblyPath, baseController) =>
            {
                var options = new StaticSiteGenerationOptions(projectPath, outputPath, assemblyPath, baseController);

                Console.WriteLine("Processing started...");
                var result = await StaticSiteBuilder.GenerateStaticSite(options);

                Console.WriteLine("Processing completed.");
                Console.WriteLine("Generated files:");

                foreach (var view in result.ViewsCompiled)
                {
                    Console.WriteLine($"-> {view}");
                }

                Console.WriteLine();
                Console.WriteLine("Errors:");

                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"-> {error}");
                }
            }, projectPathOption, outputPathOption, assemblyPathOption, baseControllerOption);

        return await rootCommand.InvokeAsync(args);
    }
}
using System.CommandLine;
using System.CommandLine.Binding;
using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Tool;

internal class StaticSiteGenerationOptionsBinder : BinderBase<StaticSiteGenerationOptions>
{
    private readonly Option<string> _projectPathOption;
    private readonly Option<string> _outputPathOption;
    private readonly Option<string?> _relativeAssemblyPathOption;
    private readonly Option<string> _baseControllerOption;
    private readonly Option<string> _defaultRouteTemplateOption;
    private readonly Option<RouteCasing> _routeCasingOption;
    private readonly Option<string> _defaultCultureOption;
    private readonly Option<bool> _useLocalizationOption;
    private readonly Option<bool> _verboseOption;

    public StaticSiteGenerationOptionsBinder()
    {
        _projectPathOption = BuildProjectPathOption();
        _outputPathOption = BuildOutputPathOption();
        _relativeAssemblyPathOption = BuildAssemblyPathOption();
        _baseControllerOption = BuildBaseControllerOption();
        _defaultRouteTemplateOption = BuildDefaultRouteTemplateOption();
        _routeCasingOption = BuildRouteCasingOption();
        _defaultCultureOption = BuildDefaultCultureOption();
        _useLocalizationOption = BuildUseLocalizationOption();
        _verboseOption = BuildVerboseOption();
    }

    internal static RootCommand BuildRootCommand()
    {
        var binder = new StaticSiteGenerationOptionsBinder();

        var rootCommand = new RootCommand(
            "This .NET tool transforms ASP.NET Core MVC websites to static sites by compiling Razor views to HTML and copying static files. "
            + Environment.NewLine + "Please look at the GitHub project for more information: "
            + "https://github.com/CamiloTerevinto/TerevintoSoftware.StaticSiteGenerator")
        {
            Name = "ssg"
        };

        rootCommand.AddOption(binder._projectPathOption);
        rootCommand.AddOption(binder._outputPathOption);
        rootCommand.AddOption(binder._relativeAssemblyPathOption);
        rootCommand.AddOption(binder._baseControllerOption);
        rootCommand.AddOption(binder._defaultRouteTemplateOption);
        rootCommand.AddOption(binder._routeCasingOption);
        rootCommand.AddOption(binder._defaultCultureOption);
        rootCommand.AddOption(binder._useLocalizationOption);
        rootCommand.AddOption(binder._verboseOption);

        rootCommand.SetHandler(async (StaticSiteGenerationOptions options) =>
        {
            Console.WriteLine("Processing started...");
            var result = await StaticSiteBuilder.GenerateStaticSite(options);

            Console.WriteLine("Processing completed.");
            Console.WriteLine($"Generated {result.ViewsCompiled.Count} files:");

            foreach (var view in result.ViewsCompiled)
            {
                Console.WriteLine($"-> {view}");
            }

            if (result.Errors.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Errors:");

                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"-> {error}");
                }

                if (!options.Verbose)
                {
                    Console.WriteLine();
                    Console.WriteLine("Use --verbose to see detailed errors.");
                }
            }
        }, binder);

        return rootCommand;
    }

    protected override StaticSiteGenerationOptions GetBoundValue(BindingContext bindingContext)
    {
        return new StaticSiteGenerationOptions(
            bindingContext.ParseResult.GetValueForOption(_projectPathOption)!,
            bindingContext.ParseResult.GetValueForOption(_outputPathOption)!,
            bindingContext.ParseResult.GetValueForOption(_relativeAssemblyPathOption),
            bindingContext.ParseResult.GetValueForOption(_baseControllerOption)!,
            bindingContext.ParseResult.GetValueForOption(_defaultRouteTemplateOption)!,
            bindingContext.ParseResult.GetValueForOption(_routeCasingOption),
            bindingContext.ParseResult.GetValueForOption(_defaultCultureOption),
            bindingContext.ParseResult.GetValueForOption(_useLocalizationOption),
            bindingContext.ParseResult.GetValueForOption(_verboseOption));
    }

    private static Option<string> BuildProjectPathOption()
    {
        var projectPathOption = new Option<string>(
            "--project",
            parseArgument: result =>
            {
                if (result.Tokens.Count != 1)
                {
                    result.ErrorMessage = "Missing project path";
                    return null!;
                }

                var projectPath = result.Tokens.Single().Value;

                if (!Directory.Exists(projectPath))
                {
                    result.ErrorMessage = $"Project path '{projectPath}' does not exist";
                    return null!;
                }

                return projectPath;
            },
            description: "The path to the project to generate the static site for.")
        {
            IsRequired = true,
        };

        return projectPathOption;
    }

    private static Option<string> BuildOutputPathOption()
    {
        var outputPathOption = new Option<string>(
            "--output",
            parseArgument: result =>
            {
                if (result.Tokens.Count != 1)
                {
                    result.ErrorMessage = "Missing output path";
                    return null!;
                }

                var outputPath = result.Tokens.Single().Value;

                if (Directory.Exists(outputPath))
                {
                    Directory.Delete(outputPath, true);
                }

                Directory.CreateDirectory(outputPath);

                return outputPath;
            },
            description: "The path to the output directory.")
        {
            IsRequired = true
        };

        return outputPathOption;
    }

    private Option<string?> BuildAssemblyPathOption()
    {
        var assemblyPathOption =
            new Option<string?>(
                "--assembly",
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        return null;
                    }

                    var relativeAssemblyPath = result.Tokens.Single().Value;

                    if (!relativeAssemblyPath.EndsWith(".dll"))
                    {
                        result.ErrorMessage = "The assembly path must refer to a .dll";
                        return null!;
                    }

                    var projectPath = result.GetValueForOption(_projectPathOption);

                    var assemblyPath = Path.Combine(projectPath!, relativeAssemblyPath);

                    if (!File.Exists(assemblyPath))
                    {
                        result.ErrorMessage = $"Assembly path '{assemblyPath}' does not exist";
                        return null;
                    }

                    return relativeAssemblyPath;
                },
                description: "The relative path to the assembly to use for the project. Defaults to /bin/Debug/net6.0/{projectName}.dll. " +
                "If given, the value must be a relative path to the project directory and point to the compiled .dll.")
            {
                IsRequired = false
            };

        return assemblyPathOption;
    }
    
    private static Option<string> BuildBaseControllerOption()
    {
        var baseControllerOption = new Option<string>(
            "--base-controller",
            () => "Home",
            description: "The name of the base controller to use for the project.");

        return baseControllerOption;
    }

    private static Option<string> BuildDefaultRouteTemplateOption()
    {
        var defaultRouteTemplateOption = new Option<string>(
            "--route-template",
            () => "{controller=Home}/{action=Index}/{id?}",
            description: "The default route template to use for the project.");

        return defaultRouteTemplateOption;
    }

    private static Option<RouteCasing> BuildRouteCasingOption()
    {
        var routeNamingFormatOption = new Option<RouteCasing>(
            "--route-casing",
            () => RouteCasing.LowerCase,
            description: "The casing convention to use for routes.");

        return routeNamingFormatOption;
    }

    private static Option<bool> BuildVerboseOption()
    {
        var verboseOption = new Option<bool>(
            "--verbose",
            () => false,
            description: "Enable verbose logging.");

        return verboseOption;
    }

    private static Option<string> BuildDefaultCultureOption()
    {
        var defaultCultureOption = new Option<string>(
            "--default-culture",
            () => "en",
            description: "The default culture to use for the project.");

        return defaultCultureOption;
    }

    private static Option<bool> BuildUseLocalizationOption()
    {
        var useLocalizationOption = new Option<bool>(
            "--use-localization",
            () => false,
            description: "Enable localization for the project.");

        return useLocalizationOption;
    }
}

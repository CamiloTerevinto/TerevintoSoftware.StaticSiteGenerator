using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Tool;

internal class GenerateSettings : CommandSettings
{
    [CommandArgument(0, "<PROJECT_PATH>")]
    [Description("The path to the project containing the ASP.NET Core MVC website.")]
    public string ProjectPath { get; set; } = string.Empty;

    [CommandOption("-o|--output")]
    [Description("The path to the output directory.")]
    public string OutputPath { get; set; } = string.Empty;

    [CommandOption("-a|--assembly")]
    [Description("The path to the compiled assembly of the project. Automatically determined if not given.")]
    public string AssemblyPath { get; set; } = string.Empty;

    [CommandOption("--base-controller")]
    [Description("The base controller of the project.")]
    [DefaultValue("Home")]
    public string BaseController { get; set; } = "Home";

    [CommandOption("--default-route-template")]
    [Description("The default route template of the project.")]
    [DefaultValue("{controller=Home}/{action=Index}/{id?}")]
    public string DefaultRouteTemplate { get; set; } = "{controller=Home}/{action=Index}/{id?}";

    [CommandOption("--route-casing")]
    [Description("The casing convention to use for routes.")]
    [DefaultValue(RouteCasing.LowerCase)]
    public RouteCasing RouteCasing { get; set; }

    [CommandOption("--default-culture")]
    [Description("The default culture of the project.")]
    [DefaultValue("en")]
    public string DefaultCulture { get; set; } = "en";

    [CommandOption("--use-localization")]
    [Description("Whether to use localization in the project.")]
    [DefaultValue(false)]
    public bool UseLocalization { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Whether to show verbose output.")]
    [DefaultValue(false)]
    public bool Verbose { get; set; }

    [CommandOption("--clear-existing-output")]
    [Description("Whether to clear the existing output directory.")]
    [DefaultValue(true)]
    public bool ClearExistingOutput { get; set; } = true;

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(ProjectPath))
        {
            return ValidationResult.Error("The project path is required.");
        }

        ProjectPath = Path.GetFullPath(ProjectPath);

        if (!Directory.Exists(ProjectPath))
        {
            return ValidationResult.Error($"The project path '{ProjectPath}' does not exist.");
        }

        if (string.IsNullOrEmpty(OutputPath))
        {
            return ValidationResult.Error("The output path is required.");
        }

        OutputPath = Path.GetFullPath(OutputPath);

        if (!string.IsNullOrEmpty(AssemblyPath))
        {
            if (!AssemblyPath.EndsWith(".dll"))
            {
                return ValidationResult.Error("The assembly path must refer to a .dll");
            }

            var assemblyPath = Path.IsPathRooted(AssemblyPath)
                ? AssemblyPath 
                : Path.Combine(ProjectPath, AssemblyPath);

            if (!File.Exists(assemblyPath))
            {
                return ValidationResult.Error($"Assembly path '{assemblyPath}' does not exist");
            }
        }
        else
        {
            AssemblyPath = AssemblyHelpers.FindAssemblyPath(ProjectPath);
        }

        return ValidationResult.Success();
    }
}

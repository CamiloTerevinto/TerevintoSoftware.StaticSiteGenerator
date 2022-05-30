using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator;

public class StaticSiteGenerationOptions
{
    /// <summary>
    /// The root directory of the site to be used for generation.
    /// </summary>
    public string ProjectPath { get; }

    /// <summary>
    /// The directory where the generated site will be placed.
    /// </summary>
    public string OutputPath { get; }

    /// <summary>
    /// The relative path to the assembly containing the site's content.
    /// </summary>
    public string RelativeAssemblyPath { get; }

    /// <summary>
    /// The name of the Base Controller (without the Controller suffix).
    /// </summary>
    public string BaseController { get; }

    /// <summary>
    /// The default route pattern, used to resolve links.
    /// </summary>
    public string DefaultRoutePattern { get; }

    /// <summary>
    /// Whether to enable verbose logs.
    /// </summary>
    public bool Verbose { get; }

    public RouteCasing RouteCasing { get; }

    /// <summary>
    /// The path to the assembly containing the site's content. 
    /// </summary>
    internal string AssemblyPath => Path.Combine(ProjectPath, RelativeAssemblyPath);

    /// <summary>
    /// Creates an instance of <see cref="StaticSiteGenerationOptions"/>.
    /// </summary>
    /// <param name="projectPath">The path to the MVC project's directory.</param>
    /// <param name="outputPath">The path to store the output.</param>
    /// <param name="relativeAssemblyPath">The path to the compiled assembly, if using a non-default path.</param>
    /// <param name="baseController">The name of the base Controller, where [Controller] is removed from the URL.</param>
    /// <param name="defaultRoutePattern">The default route pattern.</param>
    /// <param name="routeCasing">The casing convention to use for routes.</param>
    /// <param name="verbose">Whether to enable verbose logs.</param>
    public StaticSiteGenerationOptions(string projectPath, string outputPath, string? relativeAssemblyPath, string baseController, 
        string defaultRoutePattern, RouteCasing routeCasing, bool verbose)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new ArgumentNullException(nameof(projectPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentNullException(nameof(outputPath));
        }

        ProjectPath = projectPath;
        OutputPath = outputPath;
        RelativeAssemblyPath = relativeAssemblyPath ?? GetDefaultRelativeAssemblyPath(projectPath);
        BaseController = baseController;
        DefaultRoutePattern = defaultRoutePattern;
        RouteCasing = routeCasing;
        Verbose = verbose;
    }

    private static string GetDefaultRelativeAssemblyPath(string projectPath)
    {
        var separator = Path.DirectorySeparatorChar;

        var relativeAssemblyPath = $"bin{separator}Debug{separator}net6.0{separator}{new DirectoryInfo(projectPath).Name}.dll";
        var fullAssemblyPath = Path.Combine(projectPath, relativeAssemblyPath);
        
        if (File.Exists(fullAssemblyPath))
        {
            return relativeAssemblyPath;
        }

        throw new FileNotFoundException("Could not find compiled assembly.", fullAssemblyPath);
    }
}

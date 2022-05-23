namespace TerevintoSoftware.StaticSiteGenerator;

public class StaticSiteGenerationOptions
{
    public string ProjectPath { get; }
    public string OutputPath { get; }
    public string AssemblyPath { get; }
    public string BaseController { get; }

    /// <summary>
    /// Creates an instance of <see cref="StaticSiteGenerationOptions"/>. Uses the default view location format of "Views/{1}/{0}.cshtml"
    /// </summary>
    /// <param name="projectPath">The path to the MVC project's directory.</param>
    /// <param name="outputPath">The path to store the output.</param>
    /// <param name="assemblyPath">The path to the compiled assembly.</param>
    /// <param name="baseController">The name of the base Controller, where [Controller] is removed from the URL. Defaults to Home.</param>
    public StaticSiteGenerationOptions(string projectPath, string outputPath, string assemblyPath, string baseController)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new ArgumentNullException(nameof(projectPath));
        }
        else if (!Directory.Exists(projectPath))
        {
            throw new ArgumentException($"The directory '{projectPath}' does not exist.");
        }

        if (string.IsNullOrEmpty(assemblyPath))
        {
            throw new ArgumentNullException(nameof(assemblyPath));
        }
        else if (!File.Exists(assemblyPath))
        {
            throw new ArgumentException($"The file '{assemblyPath}' does not exist.");
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentNullException(nameof(outputPath));
        }

        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }

        Directory.CreateDirectory(outputPath);

        ProjectPath = projectPath;
        OutputPath = outputPath;
        AssemblyPath = assemblyPath;
        BaseController = baseController ?? "Home";
    }
}

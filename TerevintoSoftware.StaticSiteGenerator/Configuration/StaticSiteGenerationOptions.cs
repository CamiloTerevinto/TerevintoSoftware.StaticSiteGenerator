namespace TerevintoSoftware.StaticSiteGenerator;

public class StaticSiteGenerationOptions
{
    public string ProjectPath { get; }
    public string OutputPath { get; }
    public List<string> ViewsToGenerate { get; }
    public List<string> ViewsLocationFormats { get; }

    /// <summary>
    /// Creates an instance of <see cref="StaticSiteGenerationOptions"/>. Uses the default view location format of "Views/{1}/{0}.cshtml"
    /// </summary>
    /// <param name="projectPath">The path to the MVC project's directory.</param>
    /// <param name="outputPath">The path to store the output.</param>
    /// <param name="viewsToGenerate">The views to generate.</param>
    public StaticSiteGenerationOptions(string projectPath, string outputPath, IEnumerable<string> viewsToGenerate) 
        : this(projectPath, outputPath, viewsToGenerate, new List<string>())
    {
        ViewsLocationFormats.Add(Path.Combine(projectPath, "Views/{1}/{0}.cshtml"));
    }

    /// <summary>
    /// Creates an instance of <see cref="StaticSiteGenerationOptions"/>.
    /// </summary>
    /// <param name="projectPath">The path to the MVC project's directory.</param>
    /// <param name="outputPath">The path to store the output.</param>
    /// <param name="viewsToGenerate">The views to generate.</param>
    /// <param name="viewsLocationFormats">
    /// The view location formats. Each string should: 1) Start from the base of the Views directory.
    /// 2) Might have 2 placeholders: {0} for the Action name, {1} for the Controller name. 3) Should end with ".cshtml".
    /// </param>
    public StaticSiteGenerationOptions(string projectPath, string outputPath, IEnumerable<string> viewsToGenerate, IEnumerable<string> viewsLocationFormats)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new ArgumentNullException(nameof(projectPath));
        }
        else if (!Directory.Exists(projectPath))
        {
            throw new ArgumentException($"The directory '{projectPath}' does not exist.");
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentNullException(nameof(outputPath));
        }
        else if (!Directory.Exists(outputPath))
        {
            throw new ArgumentException($"The directory '{outputPath}' does not exist.");
        }

        ProjectPath = projectPath;
        OutputPath = outputPath;
        ViewsToGenerate = new List<string>(viewsToGenerate);
        ViewsLocationFormats = new List<string>();

        foreach (var viewFormat in viewsLocationFormats)
        {
            if (!viewFormat.EndsWith(".cshtml"))
            {
                throw new ArgumentException("Each view location format must end with '.cshtml'");
            }

            ViewsLocationFormats.Add(Path.Combine(projectPath, viewFormat));
        }
    }
}

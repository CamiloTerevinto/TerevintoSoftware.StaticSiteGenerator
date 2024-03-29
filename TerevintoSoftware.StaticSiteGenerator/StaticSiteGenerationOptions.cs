﻿using TerevintoSoftware.StaticSiteGenerator.Configuration;

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
    public string AssemblyPath { get; }

    /// <summary>
    /// The name of the Base Controller (without the Controller suffix).
    /// </summary>
    public string BaseController { get; }

    /// <summary>
    /// The default route pattern, used to resolve links.
    /// </summary>
    public string DefaultRoutePattern { get; }

    /// <summary>
    /// Defines whether to render views using multiple cultures.
    /// </summary>
    public bool UseLocalization { get; }

    /// <summary>
    /// Defines whether to clear the output folder or not.
    /// </summary>
    public bool ClearExistingOutput { get; }

    /// <summary>
    /// Defines the default culture to use when rendering views.
    /// </summary>
    public string DefaultCulture { get; }
    
    /// <summary>
    /// Whether to enable verbose logs.
    /// </summary>
    public bool Verbose { get; }

    /// <summary>
    /// The casing strategy to use for generating routes.
    /// </summary>
    public RouteCasing RouteCasing { get; }

    /// <summary>
    /// Creates an instance of <see cref="StaticSiteGenerationOptions"/>.
    /// </summary>
    /// <param name="projectPath">The path to the MVC project's directory.</param>
    /// <param name="outputPath">The path to store the output.</param>
    /// <param name="assemblyPath">The path to the compiled assembly, if using a non-default path.</param>
    /// <param name="baseController">The name of the base Controller, where [Controller] is removed from the URL.</param>
    /// <param name="defaultRoutePattern">The default route pattern.</param>
    /// <param name="routeCasing">The casing convention to use for routes.</param>
    /// <param name="defaultCulture">The default culture to use for rendering views.</param>
    /// <param name="useLocalization">Whether to enable localization for rendering views.</param>
    /// <param name="verbose">Whether to enable verbose logs.</param>
    public StaticSiteGenerationOptions(string projectPath, string outputPath, string assemblyPath, string baseController, 
        string defaultRoutePattern, RouteCasing routeCasing, string? defaultCulture, bool useLocalization, bool clearExistingOutput, bool verbose)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new ArgumentNullException(nameof(projectPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentNullException(nameof(outputPath));
        }

        if (string.IsNullOrWhiteSpace(assemblyPath))
        {
            throw new ArgumentNullException(nameof(assemblyPath));
        }

        ProjectPath = projectPath;
        OutputPath = outputPath;
        AssemblyPath = assemblyPath;
        BaseController = baseController;
        DefaultRoutePattern = defaultRoutePattern;
        RouteCasing = routeCasing;
        DefaultCulture = defaultCulture ?? "en";
        UseLocalization = useLocalization;
        ClearExistingOutput = clearExistingOutput;
        Verbose = verbose;
    }
}

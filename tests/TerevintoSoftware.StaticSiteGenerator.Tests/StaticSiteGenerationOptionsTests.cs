using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Tests;

public class StaticSiteGenerationOptionsTests
{
    [Test]
    public void Constructor_ShouldValidateProjectPath()
    {
        var projectPath = (string)null!;
        var outputPath = "test";
        var relativeAssemblyPath = "test";
        var baseController = "Home";
        var defaultRoutePattern = "{controller}/{action}/{id?}";
        var routeCasing = RouteCasing.LowerCase;
        var defaultCulture = "en";
        var useLocalization = false;
        var clearExistingOutput = true;
        var verbose = false;

        Assert.That(() => new StaticSiteGenerationOptions(projectPath, outputPath, relativeAssemblyPath, baseController, defaultRoutePattern, routeCasing, defaultCulture, useLocalization, clearExistingOutput, verbose), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_ShouldValidateOutputPath()
    {
        var projectPath = "test";
        var outputPath = (string)null!;
        var relativeAssemblyPath = "test";
        var baseController = "Home";
        var defaultRoutePattern = "{controller}/{action}/{id?}";
        var routeCasing = RouteCasing.LowerCase;
        var defaultCulture = "en";
        bool useLocalization = false;
        var clearExistingOutput = true;
        bool verbose = false;

        Assert.That(() => new StaticSiteGenerationOptions(projectPath, outputPath, relativeAssemblyPath, baseController, defaultRoutePattern, routeCasing, defaultCulture, useLocalization, clearExistingOutput, verbose), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_ShouldValidateRelativeAssemblyPath()
    {
        var projectPath = "test";
        var outputPath = "test";
        var relativeAssemblyPath = (string)null!;
        var baseController = "Home";
        var defaultRoutePattern = "{controller}/{action}/{id?}";
        var routeCasing = RouteCasing.LowerCase;
        var defaultCulture = "en";
        bool useLocalization = false;
        var clearExistingOutput = true;
        bool verbose = false;

        Assert.That(() => new StaticSiteGenerationOptions(projectPath, outputPath, relativeAssemblyPath, baseController, defaultRoutePattern, routeCasing, defaultCulture, useLocalization, clearExistingOutput, verbose), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_WithNoCulture_ShouldAssignEnglishAsDefault()
    {
        var projectPath = "test";
        var outputPath = "test";
        var relativeAssemblyPath = "test";
        var baseController = "Home";
        var defaultRoutePattern = "{controller}/{action}/{id?}";
        var routeCasing = RouteCasing.LowerCase;
        var defaultCulture = (string)null!;
        bool useLocalization = false;
        var clearExistingOutput = true;
        bool verbose = false;

        var options = new StaticSiteGenerationOptions(projectPath, outputPath, relativeAssemblyPath, baseController, defaultRoutePattern, routeCasing, defaultCulture, useLocalization, clearExistingOutput, verbose);

        Assert.That(options.DefaultCulture, Is.EqualTo("en"));
    }

    [Test]
    public void Constructor_ShouldAssignAllProperties()
    {
        var projectPath = "test";
        var outputPath = "test";
        var relativeAssemblyPath = "test";
        var baseController = "Home";
        var defaultRoutePattern = "{controller}/{action}/{id?}";
        var routeCasing = RouteCasing.LowerCase;
        var defaultCulture = "en";
        bool useLocalization = false;
        var clearExistingOutput = true;
        bool verbose = false;

        var options = new StaticSiteGenerationOptions(projectPath, outputPath, relativeAssemblyPath, baseController, defaultRoutePattern, routeCasing, defaultCulture, useLocalization, clearExistingOutput, verbose);
        
        Assert.Multiple(() =>
        {
            Assert.That(options.ProjectPath, Is.EqualTo(projectPath));
            Assert.That(options.OutputPath, Is.EqualTo(outputPath));
            Assert.That(options.AssemblyPath, Is.EqualTo(relativeAssemblyPath));
            Assert.That(options.BaseController, Is.EqualTo(baseController));
            Assert.That(options.DefaultRoutePattern, Is.EqualTo(defaultRoutePattern));
            Assert.That(options.RouteCasing, Is.EqualTo(routeCasing));
            Assert.That(options.DefaultCulture, Is.EqualTo(defaultCulture));
            Assert.That(options.UseLocalization, Is.EqualTo(useLocalization));
            Assert.That(options.Verbose, Is.EqualTo(verbose));
            Assert.That(options.ClearExistingOutput, Is.EqualTo(clearExistingOutput));
        });
    }
}
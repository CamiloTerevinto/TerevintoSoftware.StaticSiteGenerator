using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Tests;

public class AssemblyHelpersTests
{
    [Test]
    public void GetDefaultRelativeAssemblyPath_ShouldValidateProjectPath()
    {
        string projectPath = null!;
        Assert.That(() => AssemblyHelpers.FindAssemblyPath(projectPath), Throws.ArgumentNullException);
    }

    [Test]
    public void GetDefaultRelativeAssemblyPath_ShouldValidateDirectoryExists()
    {
        var projectPath = "abc/";
        Assert.That(() => AssemblyHelpers.FindAssemblyPath(projectPath), Throws.InstanceOf<DirectoryNotFoundException>());
    }

    [Test]
    public void GetDefaultRelativeAssemblyPath_ShouldValidateProjectFileExists()
    {
        var projectPath = Path.Combine(GetPathToSampleWebsiteFolder(), "wwwroot");
        Assert.That(() => AssemblyHelpers.FindAssemblyPath(projectPath), Throws.InstanceOf<InvalidOperationException>());
    }

    [Test]
    public void GetDefaultRelativeAssemblyPath_ShouldFindAssemblyInSampleWebsite()
    {
        var projectPath = GetPathToSampleWebsiteFolder();

        var path = AssemblyHelpers.FindAssemblyPath(projectPath);

        Assert.That(path, Is.Not.Null);
    }

    private string GetPathToSampleWebsiteFolder()
    {
        var testContext = TestContext.CurrentContext;
        var testDirectory = new DirectoryInfo(testContext.TestDirectory);

        // net8.0 => Debug => bin => TerevintoSoftware.StaticSiteGenerator.Tests => tests ==> SampleWebsite
        return Path.Combine(testDirectory.Parent!.Parent!.Parent!.Parent!.ToString(), "SampleWebsite");
    }
}
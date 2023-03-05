namespace TerevintoSoftware.StaticSiteGenerator.Tests;

public class AssemblyHelpersTests
{
    [Test]
    public void GetDefaultRelativeAssemblyPath_ShouldValidateProjectPath()
    {
        string projectPath = null!;
        Assert.That(() => AssemblyHelpers.GetDefaultRelativeAssemblyPath(projectPath), Throws.ArgumentNullException);
    }

    [Test]
    public void GetDefaultRelativeAssemblyPath_ShouldValidateAssemblyPath()
    {
        var projectPath = "test/";
        Assert.That(() => AssemblyHelpers.GetDefaultRelativeAssemblyPath(projectPath), Throws.InstanceOf<FileNotFoundException>());
    }

}
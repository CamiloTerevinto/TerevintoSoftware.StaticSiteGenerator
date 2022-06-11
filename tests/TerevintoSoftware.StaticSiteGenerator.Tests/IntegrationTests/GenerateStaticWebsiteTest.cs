using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.IntegrationTests;

[TestFixture]
public class GenerateStaticWebsiteTest
{
    [Test]
    public void GenerateStaticWebsite()
    {
        var testContext = TestContext.CurrentContext;
        var testDirectory = new DirectoryInfo(testContext.TestDirectory); 
        // net6.0 => Debug => bin => TerevintoSoftware.StaticSiteGenerator.Tests => tests ==> SampleWebsite
        var projectPath = Path.Combine(testDirectory.Parent!.Parent!.Parent!.Parent!.ToString(), "SampleWebsite");
        var outputPath = Path.Combine(testContext.WorkDirectory, "SampleWebsiteOutput");
        var assemblyPath = AssemblyHelpers.GetDefaultRelativeAssemblyPath(projectPath);

        var options = new StaticSiteGenerationOptions(projectPath, outputPath, assemblyPath, "Home", 
            "{controller=Home}/{action=Index}/{id?}", RouteCasing.LowerCase, "en", false, false);

        var result = StaticSiteBuilder.GenerateStaticSite(options).Result;
        
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewsCompiled, Has.Count.EqualTo(2));
            Assert.That(result.Errors, Is.Empty);
        });
    }
}

using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.IntegrationTests;

[TestFixture]
public class GenerateStaticWebsiteTest
{
    [Test]
    public async Task GenerateStaticWebsite()
    {
        var testContext = TestContext.CurrentContext;
        var testDirectory = new DirectoryInfo(testContext.TestDirectory); 
        // net6.0 => Debug => bin => TerevintoSoftware.StaticSiteGenerator.Tests => tests ==> SampleWebsite
        var projectPath = Path.Combine(testDirectory.Parent!.Parent!.Parent!.Parent!.ToString(), "SampleWebsite");
        var outputPath = Path.Combine(testContext.WorkDirectory, "SampleWebsiteOutput");
        var assemblyPath = AssemblyHelpers.GetDefaultRelativeAssemblyPath(projectPath);

        var options = new StaticSiteGenerationOptions(projectPath, outputPath, assemblyPath, "Home", 
            "{controller=Home}/{action=Index}/{id?}", RouteCasing.LowerCase, "en", false, false);

        var result = await StaticSiteBuilder.GenerateStaticSite(options);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.ViewsCompiled, Has.Count.EqualTo(2));
            Assert.That(result.ViewsCompiled.SingleOrDefault(x => x == "View Home/Index => index.html"), Is.Not.Null);
            Assert.That(result.ViewsCompiled.SingleOrDefault(x => x == "View Home/Privacy => privacy.html"), Is.Not.Null);
        });
    }
}

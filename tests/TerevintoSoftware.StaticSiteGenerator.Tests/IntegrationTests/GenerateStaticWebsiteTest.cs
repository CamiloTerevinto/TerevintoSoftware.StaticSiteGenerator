using HtmlAgilityPack;
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
            "{controller=Home}/{action=Index}/{id?}", RouteCasing.LowerCase, "en", true, true, false);

        var result = await StaticSiteBuilder.GenerateStaticSite(options, true);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.ViewsCompiled, Has.Count.EqualTo(4));
            Assert.That(result.ViewsCompiled.SingleOrDefault(x => x == "View Home/Index => en/index.html"), Is.Not.Null);
            Assert.That(result.ViewsCompiled.SingleOrDefault(x => x == "View Home/Index.es => es/index.html"), Is.Not.Null);
            Assert.That(result.ViewsCompiled.SingleOrDefault(x => x == "View Home/Privacy => en/privacy.html"), Is.Not.Null);
            Assert.That(result.ViewsCompiled.SingleOrDefault(x => x == "View Home/Privacy.es => es/privacy.html"), Is.Not.Null);
        });

        var expectedEnglishIndexPath = Path.Combine(outputPath, "index.html");

        var document = new HtmlDocument();
        document.Load(expectedEnglishIndexPath);

        var links = document.DocumentNode.SelectNodes("//a[starts-with(@href, '/')]");

        Assert.Multiple(() =>
        {
            Assert.That(links, Is.Not.Empty);
            Assert.That(links.Any(x => x.Attributes["href"].Value == "/index.html"), Is.True);
            Assert.That(links.Any(x => x.Attributes["href"].Value == "/en/privacy.html"), Is.True);
        });

        var expectedSpanishIndexPath = Path.Combine(outputPath, "es", "index.html");

        document = new HtmlDocument();
        document.Load(expectedSpanishIndexPath);

        links = document.DocumentNode.SelectNodes("//a[starts-with(@href, '/')]");

        Assert.Multiple(() =>
        {
            Assert.That(links, Is.Not.Empty);
            Assert.That(links.Any(x => x.Attributes["href"].Value == "/es/index.html"), Is.True);
            Assert.That(links.Any(x => x.Attributes["href"].Value == "/es/privacy.html"), Is.True);
        });
    }
}

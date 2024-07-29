using HtmlAgilityPack;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.IntegrationTests;

[TestFixture]
public class GenerateStaticWebsiteTest
{
    [Test]
    public async Task GenerateStaticWebsite()
    {
        var testContext = TestContext.CurrentContext;
        var testDirectory = new DirectoryInfo(testContext.TestDirectory);
        // net6.0 | net8.0 => Debug => bin => TerevintoSoftware.StaticSiteGenerator.Tests => tests ==> SampleWebsite
        var projectPath = Path.Combine(testDirectory.Parent!.Parent!.Parent!.Parent!.ToString(), "SampleWebsite");
        var outputPath = Path.Combine(testContext.WorkDirectory, "SampleWebsiteOutput");
        var assemblyPath = AssemblyHelpers.FindAssemblyPath(projectPath);

        var options = new StaticSiteGenerationOptions(projectPath, outputPath, assemblyPath, "Home", 
            "{controller=Home}/{action=Index}/{id?}", RouteCasing.LowerCase, "en", true, true, false);

        var result = await StaticSiteBuilder.GenerateStaticSite(options, true);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ViewsResults, Has.Count.EqualTo(2));

            var indexViewResult = result.ViewsResults.Single(x => x.ViewName == "Home/Index");
            Assert.That(indexViewResult.Results, Has.Count.EqualTo(2));
            Assert.That(indexViewResult.Results.Single(x => x.Culture == "en").GeneratedView, Does.EndWith("en" + Path.DirectorySeparatorChar + "index.html"));
            Assert.That(indexViewResult.Results.Single(x => x.Culture == "es").GeneratedView, Does.EndWith("es" + Path.DirectorySeparatorChar + "index.html"));

            var privacyViewResult = result.ViewsResults.Single(x => x.ViewName == "Home/Privacy");
            Assert.That(privacyViewResult.Results, Has.Count.EqualTo(2));
            Assert.That(privacyViewResult.Results.Single(x => x.Culture == "en").GeneratedView, Does.EndWith("en" + Path.DirectorySeparatorChar + "privacy.html"));
            Assert.That(privacyViewResult.Results.Single(x => x.Culture == "es").GeneratedView, Does.EndWith("es" + Path.DirectorySeparatorChar + "privacy.html"));
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

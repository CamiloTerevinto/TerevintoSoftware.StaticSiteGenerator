using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;

namespace TerevintoSoftware.StaticSiteGenerator.Tests;

public class UrlFormatterTests
{
    private readonly SiteAssemblyInformation _siteAssemblyInformation = new(new[] { "Index.cshtml" }, new[] { "Home", "Blog" });

    [TestCase("/", "/index.html")]
    [TestCase("/Blog", "/blog/index.html")]
    public void FormatUrl_ShouldReturnTheUrlWithIndexHtmlAdded(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog#hash", "/blog/index.html#hash")]
    public void FormatUrl_ShouldReturnTheUrlWithIndexHtmlAddedAndHash(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog/About", "/blog/about.html")]
    [TestCase("/About", "/about.html")]
    public void FormatUrl_ShouldReturnTheUrlWithHtmlExtension(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog/About#hash", "/blog/about.html#hash")]
    [TestCase("/About#hash", "/about.html#hash")]
    public void FormatUrl_ShouldReturnTheUrlWithHtmlExtensionAndHash(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }
}
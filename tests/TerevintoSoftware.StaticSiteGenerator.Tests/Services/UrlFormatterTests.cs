// Note: most of this file was auto-generated by GitHub Copilot! :)

using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Services;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.Services;

public class UrlFormatterTests
{
    private readonly SiteAssemblyInformation _siteAssemblyInformation = new(new[] { "Home", "Blog" }, Array.Empty<CultureBasedView>());
    private readonly StaticSiteGenerationOptions _lowerCaseOptions = new("test", "test", "test", "Home", "", RouteCasing.LowerCase, null, false, false);
    private readonly StaticSiteGenerationOptions _kebabCaseOptions = new("test", "test", "test", "Home", "", RouteCasing.KebabCase, null, false, false);
    private readonly StaticSiteGenerationOptions _keepCaseOptions = new("test", "test", "test", "Home", "", RouteCasing.KeepOriginal, null, false, false);

    [TestCase("/", "/index.html")]
    [TestCase("/Blog", "/blog/index.html")]
    public void FormatUrl_ShouldReturnTheUrlWithIndexHtmlAdded(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _lowerCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/About", "/about.html")]
    [TestCase("/Home/About", "/about.html")]
    [TestCase("/Home/About/us", "/about/us.html")]
    public void FormatUrl_ShouldRemoveTheHomePathFromTheUrlAndAddHtmlExtension(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _lowerCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog#hash", "/blog/index.html#hash")]
    public void FormatUrl_ShouldReturnTheUrlWithIndexHtmlAddedAndHash(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _lowerCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog/About", "/blog/about.html")]
    public void FormatUrl_ShouldReturnTheUrlWithHtmlExtension(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _lowerCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog/About#hash", "/blog/about.html#hash")]
    [TestCase("/About#hash", "/about.html#hash")]
    public void FormatUrl_ShouldReturnTheUrlWithHtmlExtensionAndHash(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _lowerCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog/Post/SomePostName", "/blog/post/somepostname.html")]
    public void FormatUrl_ShouldReturnTheUrlWithHtmlExtensionForThirdLevelUrls(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _lowerCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/", "/index.html")]
    [TestCase("/Blog", "/blog/index.html")]
    [TestCase("/Blog/PostWithLongName", "/blog/post-with-long-name.html")]
    public void FormatUrl_ShouldReturnTheUrlWithIndexHtmlAddedForKebabCase(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _kebabCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog/Post#HashWithWeirdCASing", "/blog/post.html#HashWithWeirdCASing")]
    public void FormatUrl_ShouldNotAffectTheHash(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _lowerCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/Blog/Post#HashWithWeirdCASing", "/blog/post.html#HashWithWeirdCASing")]
    public void FormatUrl_ShouldNotAffectTheHashForKebabCase(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _kebabCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }

    [TestCase("/", "/Index.html")]
    [TestCase("/Blog", "/Blog/Index.html")]
    [TestCase("/Blog/PostWithLongName", "/Blog/PostWithLongName.html")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/Blog/Post.html#HashWithWeirdCASing")]
    public void FormatUrl_ShouldReturnTheUrlWithIndexHtmlAddedForKeepCase(string inputUrl, string expectedOutputUrl)
    {
        var urlFormatter = new UrlFormatter(_siteAssemblyInformation, _keepCaseOptions);

        var outputUrl = urlFormatter.Format(inputUrl);
        Assert.That(outputUrl, Is.EqualTo(expectedOutputUrl));
    }
}
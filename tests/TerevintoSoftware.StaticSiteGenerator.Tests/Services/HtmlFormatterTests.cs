using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Services;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.Services;

[TestFixture]
public class HtmlFormatterTests
{
    const string _singleLinkWrapper = "<html><body><a href=\"{0}\">link</a></body></html>";

    [Test]
    public void EnsureHtmlIsUnmodifiedIfThereAreNoLinks()
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.LowerCase, "en", false, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = "<html><body><a href=\"https://example.com\">link</a></body></html>";
        var expectedHtml = "<html><body><a href=\"https://example.com\">link</a></body></html>";

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "en");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/index.html")]
    [TestCase("/Blog", "/blog/index.html")]
    [TestCase("/Blog#hash", "/blog/index.html#hash")]
    [TestCase("/About", "/about.html")]
    [TestCase("/Home/About", "/about.html")]
    [TestCase("/Home/About/us", "/about/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/blog/post/somepostname.html")]
    [TestCase("/About#hash", "/about.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/blog/post.html#HashWithWeirdCASing")]
    public void FormatLowerCase_NoLocalization(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.LowerCase, "en", false, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "en");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/index.html")]
    [TestCase("/Blog", "/blog/index.html")]
    [TestCase("/Blog#hash", "/blog/index.html#hash")]
    [TestCase("/About", "/about.html")]
    [TestCase("/Home/About", "/about.html")]
    [TestCase("/Home/About/us", "/about/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/blog/post/some-post-name.html")]
    [TestCase("/About#hash", "/about.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/blog/post.html#HashWithWeirdCASing")]
    public void FormatKebabCase_NoLocalization(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.KebabCase, "en", false, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "en");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/Index.html")]
    [TestCase("/Blog", "/Blog/Index.html")]
    [TestCase("/Blog#hash", "/Blog/Index.html#hash")]
    [TestCase("/About", "/About.html")]
    [TestCase("/Home/About", "/About.html")]
    [TestCase("/Home/About/us", "/About/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/Blog/Post/SomePostName.html")]
    [TestCase("/About#hash", "/About.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/Blog/Post.html#HashWithWeirdCASing")]
    public void FormatKeepOriginalCase_NoLocalization(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.KeepOriginal, "en", false, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "en");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/index.html")]
    [TestCase("/Blog", "/en/blog/index.html")]
    [TestCase("/Blog#hash", "/en/blog/index.html#hash")]
    [TestCase("/About", "/en/about.html")]
    [TestCase("/Home/About", "/en/about.html")]
    [TestCase("/Home/About/us", "/en/about/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/en/blog/post/somepostname.html")]
    [TestCase("/About#hash", "/en/about.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/en/blog/post.html#HashWithWeirdCASing")]
    public void FormatLowerCase_WithLocalization(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.LowerCase, "en", true, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en", "es" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "en");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/index.html")]
    [TestCase("/Blog", "/en/blog/index.html")]
    [TestCase("/Blog#hash", "/en/blog/index.html#hash")]
    [TestCase("/About", "/en/about.html")]
    [TestCase("/Home/About", "/en/about.html")]
    [TestCase("/Home/About/us", "/en/about/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/en/blog/post/some-post-name.html")]
    [TestCase("/About#hash", "/en/about.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/en/blog/post.html#HashWithWeirdCASing")]
    public void FormatKebabCase_WithLocalization(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.KebabCase, "en", true, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en", "es" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "en");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/Index.html")]
    [TestCase("/Blog", "/en/Blog/Index.html")]
    [TestCase("/Blog#hash", "/en/Blog/Index.html#hash")]
    [TestCase("/About", "/en/About.html")]
    [TestCase("/Home/About", "/en/About.html")]
    [TestCase("/Home/About/us", "/en/About/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/en/Blog/Post/SomePostName.html")]
    [TestCase("/About#hash", "/en/About.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/en/Blog/Post.html#HashWithWeirdCASing")]
    public void FormatKeepOriginalCase_WithLocalization(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.KeepOriginal, "en", true, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en", "es" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "en");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/es/index.html")]
    [TestCase("/Blog", "/es/blog/index.html")]
    [TestCase("/Blog#hash", "/es/blog/index.html#hash")]
    [TestCase("/About", "/es/about.html")]
    [TestCase("/Home/About", "/es/about.html")]
    [TestCase("/Home/About/us", "/es/about/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/es/blog/post/somepostname.html")]
    [TestCase("/About#hash", "/es/about.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/es/blog/post.html#HashWithWeirdCASing")]
    public void FormatLowerCase_WithLocalization_DifferentLanguage(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.LowerCase, "en", true, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en", "es" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "es");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/es/index.html")]
    [TestCase("/Blog", "/es/blog/index.html")]
    [TestCase("/Blog#hash", "/es/blog/index.html#hash")]
    [TestCase("/About", "/es/about.html")]
    [TestCase("/Home/About", "/es/about.html")]
    [TestCase("/Home/About/us", "/es/about/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/es/blog/post/some-post-name.html")]
    [TestCase("/About#hash", "/es/about.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/es/blog/post.html#HashWithWeirdCASing")]
    public void FormatKebabCase_WithLocalization_DifferentLanguage(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.KebabCase, "en", true, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en", "es" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "es");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }

    [TestCase("/", "/es/Index.html")]
    [TestCase("/Blog", "/es/Blog/Index.html")]
    [TestCase("/Blog#hash", "/es/Blog/Index.html#hash")]
    [TestCase("/About", "/es/About.html")]
    [TestCase("/Home/About", "/es/About.html")]
    [TestCase("/Home/About/us", "/es/About/us.html")]
    [TestCase("/Blog/Post/SomePostName", "/es/Blog/Post/SomePostName.html")]
    [TestCase("/About#hash", "/es/About.html#hash")]
    [TestCase("/Blog/Post#HashWithWeirdCASing", "/es/Blog/Post.html#HashWithWeirdCASing")]
    public void FormatKeepOriginalCase_WithLocalization_DifferentLanguage(string inputUrl, string expectedOutputUrl)
    {
        StaticSiteGenerationOptions options = new("test", "test", "test", "Home", "", RouteCasing.KeepOriginal, "en", true, false, false);
        SiteAssemblyInformation siteAssemblyInformation = new(new[] { "Home", "Blog" }, new CultureBasedView[] { new("Index", new[] { "en", "es" }) });
        HtmlFormatter formatter = new(siteAssemblyInformation, options);

        var inputHtml = string.Format(_singleLinkWrapper, inputUrl);
        var expectedHtml = string.Format(_singleLinkWrapper, expectedOutputUrl);

        var outputHtml = formatter.FixRelativeLinks(inputHtml, "es");
        Assert.That(outputHtml, Is.EqualTo(expectedHtml));
    }
}
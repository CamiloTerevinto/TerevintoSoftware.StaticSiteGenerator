using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.Configuration;

public class SiteAssemblyInformationTests
{
    [Test]
    public void Constructor_ShouldValidateControllerParameter()
    {
        var views = new List<CultureBasedView>();
        var controllers = (IReadOnlyCollection<string>)null!;

        Assert.That(() => new SiteAssemblyInformation(controllers, views), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_ShouldValidateViewParameter()
    {
        var views = (IReadOnlyCollection<CultureBasedView>)null!;
        var controllers = new List<string>();

        Assert.That(() => new SiteAssemblyInformation(controllers, views), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_ShouldAssignParameters()
    {
        var views = new List<CultureBasedView>();
        var controllers = new List<string>();

        var siteAssemblyInformation = new SiteAssemblyInformation(controllers, views);
        
        Assert.Multiple(() =>
        {
            Assert.That(siteAssemblyInformation.Controllers, Is.EqualTo(controllers));
            Assert.That(siteAssemblyInformation.Views, Is.EqualTo(views));
        });
    }
}

namespace TerevintoSoftware.StaticSiteGenerator.Tests;

public class StaticSiteBuilderTests
{
    [Test]
    public void GenerateStaticSite_ShouldValidateOptions()
    {
        Assert.That(() => StaticSiteBuilder.GenerateStaticSite(null!, false), Throws.ArgumentNullException);
    }
}
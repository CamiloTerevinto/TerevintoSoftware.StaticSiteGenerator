using System.Globalization;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.Utilities;

public class CultureHelperTests
{
    [Test]
    public void GetUniqueCultures_ShouldReturnCulturesFoundAndDefault()
    {
        var staticSiteGenerationOptions = new StaticSiteGenerationOptions("test", "test", "test", "Home", "en", RouteCasing.LowerCase, null, false, true, false);
        var siteAssemblyInformation = new SiteAssemblyInformation(new List<string>(), new List<CultureBasedView>
        {
            new("Home", ["es"])
        });

        var uniqueCultures = CultureHelpers.GetUniqueCultures(staticSiteGenerationOptions, siteAssemblyInformation);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(uniqueCultures, Has.Count.EqualTo(2));
            Assert.That(uniqueCultures, Has.Exactly(1).EqualTo(new CultureInfo("es")));
            Assert.That(uniqueCultures, Has.Exactly(1).EqualTo(new CultureInfo("en")));
        }
    }
}

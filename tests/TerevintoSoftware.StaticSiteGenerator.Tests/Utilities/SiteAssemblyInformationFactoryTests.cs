using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.Utilities;

public class SiteAssemblyInformationFactoryTests
{
    [ExcludeFromCodeCoverage]
    private class HomeController : Controller
    {
        public IActionResult Index() => View();
        [Route("routetest")] public IActionResult RouteTest() => View();
        [HttpGet("gettest")] public IActionResult GetTest() => View();
        [HttpGet("/ignoreroutetest")] public IActionResult IgnoreRouteTest() => View();
    }

    [ExcludeFromCodeCoverage]
    private class UnconventionalControllerrr : Controller { public IActionResult Index() => View(); } 

    private readonly List<RazorCompiledItemAttribute> _razorCompiledItemAttributes = new()
    {
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/UnconventionalControllerrr/Index.cshtml"), // Default culture
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/Home/Index.cshtml"), // Default culture
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/Home/Index.es.cshtml"), // es culture
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/Home/Index.fr-FR.cshtml"), // fr-FR culture
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/Home/RouteTest.cshtml"), // Default culture
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/Home/GetTest.cshtml"), // Default culture
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/IgnoreRouteTest.cshtml"), // Default culture
        new RazorCompiledItemAttribute(typeof(RazorPage<DateTime>), "mvc.view.1.0", "/Views/Home/ThisShouldBeIgnored.cshtml"), // Ignored
        new RazorCompiledItemAttribute(typeof(RazorPage<object>), "mvc.view.1.0", "/Views/Home/Index.abcd.fghi.cshtml") // Default culture
    };

    [Test]
    public void GetAssemblyInformation_ShouldBuildASiteAssemblyInformation()
    {
        var controllerTypes = new[] { typeof(HomeController), typeof(UnconventionalControllerrr) };
        var razorAttributes = _razorCompiledItemAttributes;
        var defaultCulture = "en";

        var result = SiteAssemblyInformationFactory.BuildAssemblyInformation(controllerTypes, razorAttributes, defaultCulture);

        var expectedHomeIndexCultures = new[] { "en", "es", "fr-FR" };
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Controllers, Has.Count.EqualTo(2));
            Assert.That(result.Views, Has.Count.EqualTo(6));
            Assert.That(result.Views.Any(x => x.ViewName == "Home/Index" && x.Cultures.SequenceEqual(expectedHomeIndexCultures)), Is.True);
        }
    }
}

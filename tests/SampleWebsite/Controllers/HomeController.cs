using Microsoft.AspNetCore.Mvc;

namespace TerevintoSoftware.StaticSiteGenerator.IntegrationTests.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}

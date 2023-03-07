using HtmlAgilityPack;
using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Services;

internal interface IHtmlFormatter
{
    string FixRelativeLinks(string html, string currentCulture);
}

internal class HtmlFormatter : IHtmlFormatter
{
    private readonly SiteAssemblyInformation _siteAssemblyInformation;
    private readonly StaticSiteGenerationOptions _staticSiteGenerationOptions;

    public HtmlFormatter(SiteAssemblyInformation siteAssemblyInformation, StaticSiteGenerationOptions staticSiteGenerationOptions)
    {
        _staticSiteGenerationOptions = staticSiteGenerationOptions;
        _siteAssemblyInformation = siteAssemblyInformation;
    }

    public string FixRelativeLinks(string html, string currentCulture)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        // Get links that start with /
        var links = document.DocumentNode.SelectNodes("//a[starts-with(@href, '/')]");

        // If there are no links to replace, just return the input
        if (links == null)
        {
            return html;
        }

        foreach (var link in links)
        {
            var href = link.Attributes["href"].Value;

            var formattedUrl = Format(href);

            if (_staticSiteGenerationOptions.UseLocalization)
            {
                if (_staticSiteGenerationOptions.DefaultCulture == currentCulture && formattedUrl.StartsWith("/index.html", StringComparison.InvariantCultureIgnoreCase))
                {
                    link.Attributes["href"].Value = formattedUrl;
                }
                else
                {
                    link.Attributes["href"].Value = $"/{currentCulture}{formattedUrl}";
                }
            }
            else
            {
                link.Attributes["href"].Value = formattedUrl;
            }
        }

        using var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        memoryStream.Position = 0;

        using var reader = new StreamReader(memoryStream);
        return reader.ReadToEnd();
    }

    private string Format(string url)
    {
        var urlParts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // For the root URL, we just return the index view
        if (urlParts.Length == 0)
        {
            return "/Index.html".ToCasing(_staticSiteGenerationOptions.RouteCasing);
        }

        var controller = _siteAssemblyInformation.Controllers.FirstOrDefault(x =>
        {
            if (urlParts[0].Contains('#'))
            {
                return x == urlParts[0].Split('#')[0];
            }

            return x == urlParts[0];
        });

        // When the first part of the URL does not refer to a controller,
        // we can assume the route refers to the root of the site.
        if (controller == null)
        {
            return AddHtmlExtension(url);
        }

        // When the controller refers to the default controller and the URL has 2 parts, the controller part is ommitted
        // i.e., /Home/About => /About.html
        if (controller == _staticSiteGenerationOptions.BaseController)
        {
            return AddHtmlExtension("/" + string.Join('/', urlParts.Skip(1)));
        }

        // When the URL refers only to a controller, it means it uses the default action.
        if (urlParts.Length == 1)
        {
            return AddIndexToControllerRoute(url, controller);
        }

        // Otherwise, the URL refers to a specific Action in a specific Controller.
        return AddHtmlExtension(url);
    }

    /// <summary>
    /// Adds /Index.html to a Controller's route (i.e., /Blog => /Blog/Index.html) and performs casing.
    /// </summary>
    /// <param name="url">The original URL (which may include a hash).</param>
    /// <param name="controller">The Controller's name.</param>
    /// <returns>The formatted URL.</returns>
    private string AddIndexToControllerRoute(string url, string controller)
    {
        var indexedUrl = $"/{controller}/Index.html".ToCasing(_staticSiteGenerationOptions.RouteCasing);
        var hashParts = url.Split('#', StringSplitOptions.RemoveEmptyEntries);

        if (hashParts.Length > 1)
        {
            indexedUrl += $"#{hashParts[1]}";
        }

        return indexedUrl;
    }

    /// <summary>
    /// Adds the .html extension to the URL, and looks for a fragment to avoid adding it twice
    /// </summary>
    /// <param name="url">The URL to add the .html extension to.</param>
    /// <returns>The updated URL.</returns>
    private string AddHtmlExtension(string url)
    {
        var parts = url.Split('#');
        var serverPart = parts[0].ToCasing(_staticSiteGenerationOptions.RouteCasing) + ".html";

        if (parts.Length == 1)
        {
            return serverPart;
        }

        return serverPart + "#" + parts[1];
    }
}
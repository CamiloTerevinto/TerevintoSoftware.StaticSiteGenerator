using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal interface IUrlFormatter
{
    string Format(string url);
}

internal class UrlFormatter : IUrlFormatter
{
    private readonly SiteAssemblyInformation _siteAssemblyInformation;
    private readonly RouteCasing _casing;

    public UrlFormatter(SiteAssemblyInformation siteAssemblyInformation, StaticSiteGenerationOptions staticSiteGenerationOptions)
    {
        _siteAssemblyInformation = siteAssemblyInformation;
        _casing = staticSiteGenerationOptions.RouteCasing;
    }

    public string Format(string url)
    {
        var urlParts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // For the root URL, we just return the index view
        if (urlParts.Length == 0)
        {
            return "/Index.html".ToCasing(_casing);
        }

        var controller = _siteAssemblyInformation.ControllersFound.FirstOrDefault(x =>
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

        // When the URL refers only to a controller, it means it uses the default action.
        if (urlParts.Length == 1)
        {
            var indexedUrl = $"/{controller}/Index.html".ToCasing(_casing);
            var hashParts = url.Split('#', StringSplitOptions.RemoveEmptyEntries);

            if (hashParts.Length > 1)
            {
                indexedUrl += $"#{hashParts[1]}";
            }

            return indexedUrl;
        }

        // Otherwise, the URL refers to a specific Action in a specific Controller.

        return AddHtmlExtension(url);
    }

    /// <summary>
    /// Adds the .html extension to the URL, and looks for a fragment to avoid adding it twice
    /// </summary>
    /// <param name="url">The URL to add the .html extension to.</param>
    /// <returns>The updated URL.</returns>
    private string AddHtmlExtension(string url)
    {
        var parts = url.Split('#');
        var serverPart = parts[0].ToCasing(_casing) + ".html";

        if (parts.Length == 1)
        {
            return serverPart;
        }

        return serverPart + "#" + parts[1];
    }
}

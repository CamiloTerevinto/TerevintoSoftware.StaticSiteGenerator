﻿using HtmlAgilityPack;
using TerevintoSoftware.StaticSiteGenerator.Internal.Services;

namespace TerevintoSoftware.StaticSiteGenerator.Services;

internal interface IHtmlFormatter
{
    string FixRelativeLinks(string html, string culture);
}

internal class HtmlFormatter : IHtmlFormatter
{
    private readonly IUrlFormatter _urlFormatter;

    public HtmlFormatter(IUrlFormatter urlFormatter)
    {
        _urlFormatter = urlFormatter;
    }

    public string FixRelativeLinks(string html, string culture)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        // get links that start with /
        var links = document.DocumentNode.SelectNodes("//a[starts-with(@href, '/')]");

        foreach (var link in links)
        {
            var href = link.Attributes["href"].Value;

            var formattedUrl = _urlFormatter.Format(href);

            link.Attributes["href"].Value = $"/{culture}{formattedUrl}";
        }

        using var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);

        return reader.ReadToEnd();
    }
}
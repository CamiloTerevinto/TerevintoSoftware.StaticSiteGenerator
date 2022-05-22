using HtmlAgilityPack;
using System.Collections.Concurrent;

namespace TerevintoSoftware.StaticSiteGenerator.Internal.Services;

internal class StaticAssetsFactory : IStaticAssetsFactory
{
    private readonly StaticAssetsResult _result;
    private readonly string _basePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticAssetsFactory"/> class.
    /// </summary>
    /// <param name="basePath">The path to the web application project.</param>
    /// <exception cref="ArgumentException"></exception>
    public StaticAssetsFactory(StaticSiteGenerationOptions options)
    {
        _result = new();
        _basePath = Path.Combine(options.ProjectPath, "wwwroot");

        if (!Directory.Exists(_basePath))
        {
            throw new InvalidOperationException($"The given path (`{_basePath}`) does not exist, is not a directory, " +
                $"or this process does not have read permission to it.");
        }
    }

    /// <summary>
    /// Adds to the builder the static assets for the given HTML that were not previously seen.
    /// </summary>
    /// <param name="html">The compiled view.</param>
    public async Task ProcessReferencesInHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var cssTask = Task.Run(() => AddLinks(_result.CssFiles, doc, "//link[@href]", "href"));
        var jsTask = Task.Run(() => AddLinks(_result.JsFiles, doc, "//script[@src]", "src"));
        var imagesTask = Task.Run(() => AddLinks(_result.Images, doc, "//img[@src]", "src"));
        var videosTask = Task.Run(() => AddLinks(_result.Videos, doc, "//video[@src]", "src"));

        await Task.WhenAll(cssTask, jsTask, imagesTask, videosTask);

        void AddLinks(ConcurrentBag<string> bag, HtmlDocument doc, string query, string attributeName)
        {
            var links = doc.DocumentNode.SelectNodes(query)?
                .Select(x => x.Attributes[attributeName].Value) ?? Array.Empty<string>();
            
            var files = links.FilterForLocalPaths().TransformToDirectoryPath(_basePath).AsParallel();

            foreach (var file in files)
            {
                bag.Add(file);
            }
        }
    }

    public StaticAssetsResult Build()
    {
        return _result;
    }
}

internal static class Extensions
{
    internal static IEnumerable<string> FilterForLocalPaths(this IEnumerable<string> paths)
    {
        return paths.Where(x => x.StartsWith("/"));
    }

    internal static IEnumerable<string> TransformToDirectoryPath(this IEnumerable<string> paths, string basePath)
    {
        return paths.Select(x =>
         {
             var index = x.IndexOf('?');
             if (index > 0)
             {
                 x = x[..index];
             }

             if (x.StartsWith('/'))
             {
                 x = x[1..];
             }

             return Path.Combine(basePath, x);
         });
    }
}

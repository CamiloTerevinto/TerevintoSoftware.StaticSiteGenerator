using System.Collections.Concurrent;

namespace TerevintoSoftware.StaticSiteGenerator.Internal;

internal class StaticAssetsResult
{
    public ConcurrentBag<string> CssFiles { get; }
    public ConcurrentBag<string> JsFiles { get; }
    public ConcurrentBag<string> Images { get; }
    public ConcurrentBag<string> Videos { get; }

    public StaticAssetsResult()
    {
        CssFiles = new ConcurrentBag<string>();
        JsFiles = new ConcurrentBag<string>();
        Images = new ConcurrentBag<string>();
        Videos = new ConcurrentBag<string>();
    }
}


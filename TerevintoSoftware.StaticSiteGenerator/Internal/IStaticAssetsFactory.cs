namespace TerevintoSoftware.StaticSiteGenerator.Internal;

internal interface IStaticAssetsFactory
{
    Task ProcessReferencesInHtml(string html);
    StaticAssetsResult Build();
}
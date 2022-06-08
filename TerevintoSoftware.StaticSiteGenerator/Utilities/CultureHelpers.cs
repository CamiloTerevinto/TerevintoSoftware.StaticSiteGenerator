using System.Globalization;
using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Utilities;

internal static class CultureHelpers
{
    /// <summary>
    /// Retrieves the unique cultures that were found by looking at Views in the assembly.
    /// </summary>
    internal static IList<CultureInfo> GetUniqueCultures(StaticSiteGenerationOptions staticSiteOptions, SiteAssemblyInformation siteAssemblyInformation)
    {
        return siteAssemblyInformation.Views
            .SelectMany(v => v.Cultures)
            .Prepend(staticSiteOptions.DefaultCulture)
            .Distinct()
            .Select(x => new CultureInfo(x))
            .ToArray();
    }
}

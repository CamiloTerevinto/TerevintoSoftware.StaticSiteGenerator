namespace TerevintoSoftware.StaticSiteGenerator.Configuration;

/// <summary>
/// The casing to use when generating the routes.
/// </summary>
public enum RouteCasing
{
    /// <summary>
    /// Converts all routes to lowercase. Default, as it helps with casing in case-sensitive systems.
    /// </summary>
    LowerCase = 0,

    /// <summary>
    /// Converts all routes to kebab-case when possible (from PascalCase).
    /// </summary>
    KebabCase = 1,

    /// <summary>
    /// Keeps the original casing.
    /// </summary>
    KeepOriginal = 2,
}

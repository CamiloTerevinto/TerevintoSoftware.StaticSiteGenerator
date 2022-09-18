using System.Text.RegularExpressions;
using TerevintoSoftware.StaticSiteGenerator.Configuration;

namespace TerevintoSoftware.StaticSiteGenerator.Utilities;

internal static class StringExtensions
{
    /// <summary>
    /// Changes the casing of a string according to the specified format.
    /// </summary>
    /// <param name="value">The string to update.</param>
    /// <param name="casing">The desired casing.</param>
    /// <returns>The updated value.</returns>
    internal static string ToCasing(this string value, RouteCasing casing)
    {
        return casing switch
        {
            RouteCasing.LowerCase => value.ToLower(),
            RouteCasing.KebabCase => ToKebabCase(value),
            _ => value
        };
    }

    /// <summary>
    /// Converts a string to a kebab-cased representation.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The kebab-cased value.</returns>
    private static string ToKebabCase(string value)
    {
        value = Regex.Replace(value, "([a-zA-Z])([A-Z])", "$1-$2").ToLower();
        value = Regex.Replace(value, "(\\d+)", "-$1-");

        return value.TrimEnd('-');
    }
}

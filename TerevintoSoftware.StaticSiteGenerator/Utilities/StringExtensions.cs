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
        // 1. Separate words that have numbers (i.e., B2c => -b2c-)
        var replacedValue = Regex.Replace(value, "([A-Z]?\\d+[a-z]+)", "-$1-");

        if (replacedValue == value)
        {
            // 2. If the previous Regex didn't match, just split words by numbers (i.e., b2c => b-2-c).
            replacedValue = Regex.Replace(value, "(\\d+)", "-$1-");
        }

        // 3. Separate SentenceCased words (i.e, HelloWorld => Hello-World)
        replacedValue = Regex.Replace(replacedValue, "([a-zA-Z])([A-Z])", "$1-$2");

        // 4. Remove a potential suffix/prefix '-'  from step 1 or 2, and convert the result to lowercase.
        return replacedValue.Trim('-').ToLower();
    }
}

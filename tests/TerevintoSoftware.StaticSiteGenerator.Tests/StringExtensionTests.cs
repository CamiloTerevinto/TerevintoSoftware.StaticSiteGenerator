using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Tests;

internal class StringExtensionTests
{
    [TestCase("HelloWorld", "helloworld")]
    [TestCase("test", "test")]
    public void ToCasing_ShouldConvertAStringToLowerCase(string input, string output)
    {
        Assert.That(input.ToCasing(RouteCasing.LowerCase), Is.EqualTo(output));
    }

    [TestCase("HelloWorld", "hello-world")]
    [TestCase("test", "test")]
    public void ToCasing_ShouldConvertAStringToKebabCase(string input, string output)
    {
        Assert.That(input.ToCasing(RouteCasing.KebabCase), Is.EqualTo(output));
    }

    [TestCase("HelloWorld", "HelloWorld")]
    [TestCase("test", "test")]
    public void ToCasing_ShouldKeepOriginalStringCase(string input, string output)
    {
        Assert.That(input.ToCasing(RouteCasing.KeepOriginal), Is.EqualTo(output));
    }
}

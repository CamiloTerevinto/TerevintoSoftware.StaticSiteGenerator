using TerevintoSoftware.StaticSiteGenerator.Configuration;
using TerevintoSoftware.StaticSiteGenerator.Utilities;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.Utilities;

internal class StringExtensionTests
{
    [TestCase("HelloWorld", "helloworld")]
    [TestCase("test", "test")]
    [TestCase("TEST", "test")]
    [TestCase("tES2t", "tes2t")]
    public void ToCasing_ShouldConvertAStringToLowerCase(string input, string output)
    {
        Assert.That(input.ToCasing(RouteCasing.LowerCase), Is.EqualTo(output));
    }

    [TestCase("HelloWorld", "hello-world")]
    [TestCase("HElloWorld", "h-ello-world")]
    [TestCase("Hello22World", "hello-22-world")]
    [TestCase("test", "test")]
    [TestCase("test22", "test-22")]
    [TestCase("HelloW22orld", "hello-w22orld")]
    [TestCase("A22Hours", "a-22-hours")]
    [TestCase("22Hours", "22-hours")]
    [TestCase("22hours", "22hours")]
    public void ToCasing_ShouldConvertAStringToKebabCase(string input, string output)
    {
        Assert.That(input.ToCasing(RouteCasing.KebabCase), Is.EqualTo(output));
    }

    [TestCase("HelloWorld", "HelloWorld")]
    [TestCase("test", "test")]
    [TestCase("TEST", "TEST")]
    [TestCase("tES2t", "tES2t")]
    public void ToCasing_ShouldKeepOriginalStringCase(string input, string output)
    {
        Assert.That(input.ToCasing(RouteCasing.KeepOriginal), Is.EqualTo(output));
    }
}

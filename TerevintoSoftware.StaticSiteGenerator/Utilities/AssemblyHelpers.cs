using System.Xml;

namespace TerevintoSoftware.StaticSiteGenerator.Utilities;

public static class AssemblyHelpers
{
    public static string FindAssemblyPath(string projectPath)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            throw new ArgumentNullException(nameof(projectPath));
        }

        var projectFile = Directory.GetFiles(projectPath, "*.csproj").FirstOrDefault();

        if (string.IsNullOrEmpty(projectFile))
        {
            throw new InvalidOperationException("There is no .csproj file available at the path specified");
        }

        var doc = new XmlDocument();
        doc.Load(projectFile);
        var targetFramework = doc.SelectSingleNode("//TargetFramework")?.InnerText ?? doc.SelectSingleNode("//TargetFrameworks")?.InnerText;

        if (string.IsNullOrEmpty(targetFramework))
        {
            throw new InvalidOperationException("Could not find the target framework in the .csproj file");
        }

        if (targetFramework.Contains(';'))
        {
            targetFramework = targetFramework[..targetFramework.IndexOf(';')];
        }

        var projectName = new DirectoryInfo(projectPath).Name;
        var releaseAssemblyPath = Path.Combine(projectPath, "bin", "Release", targetFramework, $"{projectName}.dll");

        if (File.Exists(releaseAssemblyPath))
        {
            return releaseAssemblyPath;
        }

        var debugAssemblyPath = Path.Combine(projectPath, "bin", "Debug", targetFramework, $"{projectName}.dll");
        if (File.Exists(debugAssemblyPath))
        {
            return debugAssemblyPath;
        }

        throw new FileNotFoundException($"Could not find a compiled assembly under '{releaseAssemblyPath}' nor under '{debugAssemblyPath}'.");
    }
}

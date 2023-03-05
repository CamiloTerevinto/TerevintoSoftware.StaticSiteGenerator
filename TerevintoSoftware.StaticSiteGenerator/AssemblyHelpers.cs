namespace TerevintoSoftware.StaticSiteGenerator;

public static class AssemblyHelpers
{
    public static string GetDefaultRelativeAssemblyPath(string projectPath)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            throw new ArgumentNullException(nameof(projectPath));
        }

        var separator = Path.DirectorySeparatorChar;

        var relativeAssemblyPath = $"bin{separator}Debug{separator}net6.0{separator}{new DirectoryInfo(projectPath).Name}.dll";
        var fullAssemblyPath = Path.Combine(projectPath, relativeAssemblyPath);

        if (File.Exists(fullAssemblyPath))
        {
            return relativeAssemblyPath;
        }

        throw new FileNotFoundException("Could not find the compiled assembly.", fullAssemblyPath);
    }
}

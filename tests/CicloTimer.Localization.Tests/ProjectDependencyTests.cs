namespace CicloTimer.Localization.Tests;

public sealed class ProjectDependencyTests
{
    private static readonly string RepositoryRoot = GetRepositoryRoot();
    private static readonly string LocalizationProjectPath = Path.Combine(
        RepositoryRoot,
        "locales",
        "CicloTimer.Localization");

    private static readonly string LocalizationTestsProjectPath = Path.Combine(
        RepositoryRoot,
        "tests",
        "CicloTimer.Localization.Tests");

    [Fact]
    public void LocalizationProjectUsesNet9AndNotWindowsTarget()
    {
        var project = ReadLocalizationProject();

        Assert.Contains("<TargetFramework>net9.0</TargetFramework>", project);
        Assert.DoesNotContain("net9.0-windows", project);
    }

    [Fact]
    public void LocalizationProjectDoesNotReferenceForbiddenProjectsOrFrameworks()
    {
        var project = ReadLocalizationProject();

        Assert.DoesNotContain("CicloTimer.Core", project);
        Assert.DoesNotContain("CicloTimer.Bridge", project);
        Assert.DoesNotContain("ciclotimer.csproj", project);
        Assert.DoesNotContain("WindowsBase", project);
        Assert.DoesNotContain("PresentationFramework", project);
        Assert.DoesNotContain("UseWPF", project);
        Assert.DoesNotContain("System.Windows", project);
    }

    [Fact]
    public void TestProjectReferencesOnlyLocalizationProject()
    {
        var project = File.ReadAllText(Path.Combine(
            LocalizationTestsProjectPath,
            "CicloTimer.Localization.Tests.csproj"));

        Assert.Contains("..\\..\\locales\\CicloTimer.Localization\\CicloTimer.Localization.csproj", project);
        Assert.DoesNotContain("CicloTimer.Core", project);
        Assert.DoesNotContain("CicloTimer.Bridge", project);
        Assert.DoesNotContain("ciclotimer.csproj", project);
        Assert.DoesNotContain("WindowsBase", project);
        Assert.DoesNotContain("PresentationFramework", project);
        Assert.DoesNotContain("UseWPF", project);
    }

    [Fact]
    public void LocalizationProjectDoesNotContainJsonXmlOrResxFiles()
    {
        var forbiddenFiles = Directory.EnumerateFiles(LocalizationProjectPath, "*.*", SearchOption.AllDirectories)
            .Where(IsSourceFile)
            .Where(path =>
                path.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.Empty(forbiddenFiles);
    }

    [Fact]
    public void LocalizationProjectDoesNotContainFutureLanguageFolders()
    {
        var localesPath = Path.Combine(LocalizationProjectPath, "Locales");

        Assert.True(Directory.Exists(Path.Combine(localesPath, "It")));
        Assert.False(Directory.Exists(Path.Combine(localesPath, "En")));
        Assert.False(Directory.Exists(Path.Combine(localesPath, "Fr")));
        Assert.False(Directory.Exists(Path.Combine(localesPath, "Es")));
    }

    [Fact]
    public void LocalizationCodeDoesNotUseForbiddenRuntimeDependencies()
    {
        var source = string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(LocalizationProjectPath, "*.cs", SearchOption.AllDirectories)
                .Where(IsSourceFile)
                .Select(File.ReadAllText));

        Assert.DoesNotContain("CultureInfo", source);
        Assert.DoesNotContain("System.Windows", source);
        Assert.DoesNotContain("AutomationProperties", source);
        Assert.DoesNotContain("NVDA", source);
        Assert.DoesNotContain("CicloTimer.Core", source);
        Assert.DoesNotContain("TimerState.", source);
        Assert.DoesNotContain("TimerEvent.", source);
        Assert.DoesNotContain("TimerError.", source);
        Assert.DoesNotContain("TimerCommandResult", source);
    }

    private static string ReadLocalizationProject()
    {
        return File.ReadAllText(Path.Combine(
            LocalizationProjectPath,
            "CicloTimer.Localization.csproj"));
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CicloTimer.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("Repository root with CicloTimer.sln was not found.");
    }

    private static bool IsSourceFile(string path)
    {
        var normalized = path.Replace(Path.DirectorySeparatorChar, '/');
        return !normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase) &&
            !normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase);
    }
}

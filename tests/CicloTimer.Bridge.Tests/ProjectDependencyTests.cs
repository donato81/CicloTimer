using Xunit;

namespace CicloTimer.Bridge.Tests;

public sealed class ProjectDependencyTests
{
    [Fact]
    public void BridgeProject_UsesExpectedTargetAndReferences()
    {
        var root = FindRepositoryRoot();
        var projectText = File.ReadAllText(Path.Combine(root, "view-models", "CicloTimer.Bridge", "CicloTimer.Bridge.csproj"));

        Assert.Contains("<TargetFramework>net9.0</TargetFramework>", projectText);
        Assert.DoesNotContain("net9.0-windows", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"..\..\models\CicloTimer.Core\CicloTimer.Core.csproj", projectText);
        Assert.Contains(@"..\..\locales\CicloTimer.Localization\CicloTimer.Localization.csproj", projectText);
        Assert.DoesNotContain("ciclotimer.csproj", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PresentationFramework", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("WindowsBase", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("UIAutomation", projectText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BridgeTestsProject_ReferencesOnlyAllowedProjects()
    {
        var root = FindRepositoryRoot();
        var projectText = File.ReadAllText(Path.Combine(root, "tests", "CicloTimer.Bridge.Tests", "CicloTimer.Bridge.Tests.csproj"));

        Assert.Contains("<TargetFramework>net9.0</TargetFramework>", projectText);
        Assert.Contains(@"..\..\view-models\CicloTimer.Bridge\CicloTimer.Bridge.csproj", projectText);
        Assert.Contains(@"..\..\models\CicloTimer.Core\CicloTimer.Core.csproj", projectText);
        Assert.Contains(@"..\..\locales\CicloTimer.Localization\CicloTimer.Localization.csproj", projectText);
        Assert.DoesNotContain("ciclotimer.csproj", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PresentationFramework", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("WindowsBase", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("UIAutomation", projectText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BridgeSource_DoesNotContainForbiddenDependenciesOrKeyGenerationPatterns()
    {
        var root = FindRepositoryRoot();
        var bridgeDirectory = Path.Combine(root, "view-models", "CicloTimer.Bridge");
        var sourceText = string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(bridgeDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !IsBuildArtifactPath(path))
                .Select(File.ReadAllText));

        Assert.DoesNotContain("System.Windows", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DispatcherTimer", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("System.Timers.Timer", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("AutomationProperties", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("NVDA", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("UIAutomation", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DllImport", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SoundPlayer", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("NAudio", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("MediaPlayer", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Console.Beep", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("System.Reflection", sourceText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(".ToString()", sourceText, StringComparison.Ordinal);
        Assert.DoesNotContain("GetType()", sourceText, StringComparison.Ordinal);
        Assert.DoesNotContain("typeof(", sourceText, StringComparison.Ordinal);
    }

    [Fact]
    public void Repository_DoesNotContainSrcFolder()
    {
        var root = FindRepositoryRoot();

        Assert.False(Directory.Exists(Path.Combine(root, "src")));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CicloTimer.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root was not found.");
    }

    private static bool IsBuildArtifactPath(string path)
    {
        var normalizedPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        return normalizedPath.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);
    }
}

using System.IO;

namespace CicloTimer.Presentation.Tests;

public sealed class ProjectDependencyTests
{
    [Fact]
    public void WpfProjectLivesUnderViews()
    {
        var root = FindRepositoryRoot();

        Assert.True(File.Exists(Path.Combine(root, "views", "ciclotimer", "ciclotimer.csproj")));
        Assert.False(File.Exists(Path.Combine(root, "ciclotimer.csproj")));
    }

    [Fact]
    public void ViewModelsProjectDoesNotReferenceForbiddenLayers()
    {
        var root = FindRepositoryRoot();
        var projectText = File.ReadAllText(Path.Combine(root, "view-models", "CicloTimer.ViewModels", "CicloTimer.ViewModels.csproj"));

        Assert.Contains("<TargetFramework>net9.0-windows</TargetFramework>", projectText);
        Assert.Contains(@"..\..\services\CicloTimer.App\CicloTimer.App.csproj", projectText);
        Assert.Contains(@"..\..\locales\CicloTimer.Localization\CicloTimer.Localization.csproj", projectText);
        Assert.DoesNotContain("CicloTimer.Core.csproj", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("CicloTimer.Audio.csproj", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ciclotimer.csproj", projectText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ViewModelSourceDoesNotUseForbiddenDependenciesOrTimers()
    {
        var root = FindRepositoryRoot();
        var source = ReadSource(Path.Combine(root, "view-models", "CicloTimer.ViewModels"));

        string[] forbidden =
        [
            "CicloTimer.Core",
            "CicloTimer.Audio",
            "TimerEngine",
            "AudioService",
            "TimerBridgeAdapter",
            "SystemActionDispatcher",
            "Dispatcher.Invoke",
            "DispatcherTimer",
            "System.Timers.Timer",
            "System.Threading.Timer",
            "Task.Delay"
        ];

        foreach (var forbiddenText in forbidden)
        {
            Assert.DoesNotContain(forbiddenText, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ViewsDoNotUseTextBoxesPollingOrUiTimers()
    {
        var root = FindRepositoryRoot();
        var source = ReadSource(Path.Combine(root, "views", "ciclotimer"));

        string[] forbidden =
        [
            "TextBox",
            "Dispatcher.Invoke",
            "DispatcherTimer",
            "System.Timers.Timer",
            "System.Threading.Timer",
            "Task.Delay",
            "LiveSetting",
            "LiveRegion"
        ];

        foreach (var forbiddenText in forbidden)
        {
            Assert.DoesNotContain(forbiddenText, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void AppLayerDoesNotContainUiCompositionRoot()
    {
        var root = FindRepositoryRoot();
        var appSource = ReadSource(Path.Combine(root, "services", "CicloTimer.App"));

        Assert.DoesNotContain("MainTimerViewModel", appSource, StringComparison.Ordinal);
        Assert.DoesNotContain("MainWindow", appSource, StringComparison.Ordinal);
        Assert.DoesNotContain("SynchronizationContextUiDispatcher", appSource, StringComparison.Ordinal);
    }

    [Fact]
    public void NumericStepControlUsesTextBlockAndTwoWayValueMetadata()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "views", "ciclotimer", "Controls", "NumericStepControl.xaml"));
        var code = File.ReadAllText(Path.Combine(root, "views", "ciclotimer", "Controls", "NumericStepControl.xaml.cs"));

        Assert.Contains("TextBlock", xaml, StringComparison.Ordinal);
        Assert.DoesNotContain("TextBox", xaml, StringComparison.Ordinal);
        Assert.Contains("BindsTwoWayByDefault", code, StringComparison.Ordinal);
        Assert.Contains("Clamp", code, StringComparison.Ordinal);
    }

    private static string ReadSource(string directory)
    {
        return string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                .Where(path => !IsBuildArtifactPath(path))
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(File.ReadAllText));
    }

    private static bool IsBuildArtifactPath(string path)
    {
        var normalizedPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        return normalizedPath.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);
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
}

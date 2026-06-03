namespace CicloTimer.Audio.Tests;

public sealed class ProjectDependencyTests
{
    [Fact]
    public void AudioProjectUsesWindowsTarget()
    {
        string projectText = File.ReadAllText(GetAudioProjectPath());

        Assert.Contains("<TargetFramework>net9.0-windows</TargetFramework>", projectText);
    }

    [Fact]
    public void AudioProjectDoesNotReferenceForbiddenProjects()
    {
        string projectText = File.ReadAllText(GetAudioProjectPath());

        Assert.DoesNotContain("CicloTimer.Core.csproj", projectText);
        Assert.DoesNotContain("CicloTimer.Localization.csproj", projectText);
        Assert.DoesNotContain("CicloTimer.Bridge.csproj", projectText);
        Assert.DoesNotContain("ciclotimer.csproj", projectText);
        Assert.DoesNotContain("tests", projectText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ProjectReference", projectText);
    }

    [Fact]
    public void AudioSourcesDoNotContainForbiddenUiOrAudioDependencies()
    {
        string combinedSources = ReadAudioSources();

        Assert.DoesNotContain("System.Windows", combinedSources);
        Assert.DoesNotContain("PresentationFramework", combinedSources);
        Assert.DoesNotContain("UIAutomation", combinedSources);
        Assert.DoesNotContain("ICommand", combinedSources);
        Assert.DoesNotContain("INotifyPropertyChanged", combinedSources);
        Assert.DoesNotContain("NAudio", combinedSources);
    }

    [Fact]
    public void AudioSourcesDoNotContainForbiddenExternalManipulation()
    {
        string combinedSources = ReadAudioSources();

        Assert.DoesNotContain("Process.", combinedSources);
        Assert.DoesNotContain("GetProcesses", combinedSources);
        Assert.DoesNotContain("Kill(", combinedSources);
        Assert.DoesNotContain("C:\\Windows\\Media", combinedSources, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SetMasterVolume", combinedSources);
        Assert.DoesNotContain("EndpointVolume", combinedSources);
    }

    [Fact]
    public void RepositoryDoesNotContainSrcFolder()
    {
        string repositoryRoot = GetRepositoryRoot();

        Assert.False(Directory.Exists(Path.Combine(repositoryRoot, "src")));
    }

    [Fact]
    public void WindowsAudioPlayerUsesSoundPlayer()
    {
        string path = Path.Combine(GetRepositoryRoot(), "services", "CicloTimer.Audio", "WindowsAudioPlayer.cs");
        string source = File.ReadAllText(path);

        Assert.Contains("System.Media", source);
        Assert.Contains("SoundPlayer", source);
    }

    private static string ReadAudioSources()
    {
        string audioDirectory = Path.Combine(GetRepositoryRoot(), "services", "CicloTimer.Audio");
        return string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(audioDirectory, "*.cs", SearchOption.AllDirectories)
                .Select(File.ReadAllText));
    }

    private static string GetAudioProjectPath()
    {
        return Path.Combine(GetRepositoryRoot(), "services", "CicloTimer.Audio", "CicloTimer.Audio.csproj");
    }

    private static string GetRepositoryRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }
}

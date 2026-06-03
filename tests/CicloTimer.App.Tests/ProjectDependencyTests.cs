using System.Xml.Linq;
using CicloTimer.App;
using CicloTimer.Audio;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests;

public sealed class ProjectDependencyTests
{
    [Fact]
    public void AppProjectUsesNet9Windows()
    {
        var project = XDocument.Load(ProjectPaths.AppProject);

        Assert.Equal("net9.0-windows", project.Descendants("TargetFramework").Single().Value);
    }

    [Fact]
    public void AppTestsProjectUsesNet9Windows()
    {
        var project = XDocument.Load(ProjectPaths.AppTestsProject);

        Assert.Equal("net9.0-windows", project.Descendants("TargetFramework").Single().Value);
    }

    [Fact]
    public void AppReferencesOnlyBridgeAndAudio()
    {
        var references = ProjectReferenceIncludes(ProjectPaths.AppProject);

        Assert.Contains(@"..\..\view-models\CicloTimer.Bridge\CicloTimer.Bridge.csproj", references);
        Assert.Contains(@"..\CicloTimer.Audio\CicloTimer.Audio.csproj", references);
        Assert.DoesNotContain(references, reference => reference.Contains("CicloTimer.Core", StringComparison.Ordinal));
        Assert.DoesNotContain(references, reference => reference.Contains("CicloTimer.Localization", StringComparison.Ordinal));
        Assert.DoesNotContain(references, reference => reference.Contains("ciclotimer.csproj", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(references, reference => reference.Contains("tests", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(2, references.Count);
    }

    [Fact]
    public void AppTestsReferenceOnlyAppProject()
    {
        var references = ProjectReferenceIncludes(ProjectPaths.AppTestsProject);

        Assert.Equal(new[] { @"..\..\services\CicloTimer.App\CicloTimer.App.csproj" }, references);
    }

    [Fact]
    public void AppDoesNotContainUiOrTimerForbiddenApis()
    {
        var source = ProjectPaths.ReadAppSource();
        string[] forbidden =
        [
            "System.Windows",
            "PresentationFramework",
            "UIAutomation",
            "DispatcherTimer",
            "System.Timers.Timer",
            "System.Threading.Timer",
            "Task.Delay",
            "ICommand",
            "INotifyPropertyChanged"
        ];

        foreach (var forbiddenText in forbidden)
        {
            Assert.DoesNotContain(forbiddenText, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void AppDoesNotContainForbiddenFolders()
    {
        Assert.False(Directory.Exists(Path.Combine(ProjectPaths.RepositoryRoot, "src")));
        Assert.False(Directory.Exists(Path.Combine(ProjectPaths.RepositoryRoot, "orchestrators")));
    }

    [Fact]
    public void AppDoesNotUseFreeStringInterpretationForSystemActions()
    {
        var source = ProjectPaths.ReadAppSource();

        Assert.DoesNotContain(".ToString()", source, StringComparison.Ordinal);
        Assert.DoesNotContain("GetType()", source, StringComparison.Ordinal);
        Assert.DoesNotContain("typeof(SystemActionRequest)", source, StringComparison.Ordinal);
    }

    [Fact]
    public void AppUsesRealBridgeAndAudioTypes()
    {
        Assert.Equal(typeof(TimerBridgeUpdate), typeof(ITimerBridgePort).GetMethod(nameof(ITimerBridgePort.Start))!.ReturnType);
        Assert.Equal(typeof(TimerInput), typeof(ITimerBridgePort).GetMethod(nameof(ITimerBridgePort.Configure))!.GetParameters()[0].ParameterType);
        Assert.Equal(typeof(int), typeof(ITimerBridgePort).GetMethod(nameof(ITimerBridgePort.Tick))!.GetParameters()[0].ParameterType);
        Assert.Equal(typeof(AudioServiceResult), typeof(IAudioServicePort).GetMethod(nameof(IAudioServicePort.StartFinalAlertSound))!.ReturnType);
    }

    [Fact]
    public void AppDoesNotUseObjectInPortsWhenRealTypesExist()
    {
        var portMethods = typeof(ITimerBridgePort).GetMethods()
            .Concat(typeof(IAudioServicePort).GetMethods());

        foreach (var method in portMethods)
        {
            Assert.NotEqual(typeof(object), method.ReturnType);
            Assert.DoesNotContain(method.GetParameters(), parameter => parameter.ParameterType == typeof(object));
        }
    }

    private static List<string> ProjectReferenceIncludes(string projectPath)
    {
        return XDocument.Load(projectPath)
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value ?? string.Empty)
            .Where(value => value.Length > 0)
            .ToList();
    }
}

internal static class ProjectPaths
{
    public static string RepositoryRoot { get; } = FindRepositoryRoot();
    public static string AppProject { get; } = Path.Combine(RepositoryRoot, "services", "CicloTimer.App", "CicloTimer.App.csproj");
    public static string AppTestsProject { get; } = Path.Combine(RepositoryRoot, "tests", "CicloTimer.App.Tests", "CicloTimer.App.Tests.csproj");

    public static string ReadAppSource()
    {
        var appDirectory = Path.Combine(RepositoryRoot, "services", "CicloTimer.App");
        var files = Directory.EnumerateFiles(appDirectory, "*.cs", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.Ordinal);

        return string.Join(Environment.NewLine, files.Select(File.ReadAllText));
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

        throw new InvalidOperationException("Repository root not found for tests.");
    }
}

using CicloTimer.App;
using CicloTimer.Audio;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests;

public sealed class TimerAppOrchestratorInitializationTests
{
    [Fact]
    public void ConstructorRequiresBridgePort()
    {
        var audio = new FakeAudioServicePort();

        Assert.Throws<ArgumentNullException>(() =>
            new TimerAppOrchestrator(null!, audio));
    }

    [Fact]
    public void ConstructorRequiresAudioPort()
    {
        var bridge = new FakeTimerBridgePort();

        Assert.Throws<ArgumentNullException>(() =>
            new TimerAppOrchestrator(bridge, (IAudioServicePort)null!));
    }

    [Fact]
    public void InitializationRequestsInitialModelFromBridge()
    {
        var bridge = new FakeTimerBridgePort();
        var audio = new FakeAudioServicePort();

        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        Assert.Equal(1, bridge.GetCurrentUpdateCalls);
    }

    [Fact]
    public void CurrentStateContainsInitialModel()
    {
        var initialModel = TestModelFactory.Create("initial");
        var bridge = new FakeTimerBridgePort { CurrentModel = initialModel };
        var audio = new FakeAudioServicePort();

        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        Assert.Same(initialModel, orchestrator.CurrentState.CurrentModel);
    }

    [Fact]
    public void CurrentStateIsReadOnlySnapshot()
    {
        var property = typeof(TimerAppOrchestrator).GetProperty(nameof(TimerAppOrchestrator.CurrentState));

        Assert.NotNull(property);
        Assert.False(property!.CanWrite);
        Assert.True(typeof(TimerAppState).IsAssignableTo(typeof(object)));
    }

    [Fact]
    public void InitializationDoesNotCallAudio()
    {
        var bridge = new FakeTimerBridgePort();
        var audio = new FakeAudioServicePort();

        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        Assert.Empty(audio.Calls);
    }

    [Fact]
    public void InitializationDoesNotGenerateTick()
    {
        var bridge = new FakeTimerBridgePort();
        var audio = new FakeAudioServicePort();

        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        Assert.Equal(0, bridge.TickCalls);
    }

    [Fact]
    public void InitializationDoesNotCreateRealTimer()
    {
        var fields = typeof(TimerAppOrchestrator).GetFields(
            System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Public);

        Assert.DoesNotContain(fields, field =>
            field.FieldType.FullName is "System.Timers.Timer" or "System.Threading.Timer");
    }
}

internal sealed class FakeTimerBridgePort : ITimerBridgePort
{
    private int modelSequence;

    public TimerDisplayModel CurrentModel { get; set; } = TestModelFactory.Create("initial");

    public Queue<IReadOnlyList<SystemActionRequest>> ActionsByCommand { get; } = new();

    public int GetCurrentUpdateCalls { get; private set; }
    public int ConfigureCalls { get; private set; }
    public int StartCalls { get; private set; }
    public int PauseCalls { get; private set; }
    public int ResumeCalls { get; private set; }
    public int ResetCalls { get; private set; }
    public int TickCalls { get; private set; }
    public TimerInput? LastInput { get; private set; }
    public int? LastElapsedSeconds { get; private set; }
    public bool ThrowOnNextCommand { get; set; }

    public TimerBridgeUpdate GetCurrentUpdate()
    {
        GetCurrentUpdateCalls++;
        return new TimerBridgeUpdate(CurrentModel, Array.Empty<SystemActionRequest>());
    }

    public TimerBridgeUpdate Configure(TimerInput input)
    {
        ConfigureCalls++;
        LastInput = input;
        return ExecuteCommand("configure");
    }

    public TimerBridgeUpdate Start()
    {
        StartCalls++;
        return ExecuteCommand("start");
    }

    public TimerBridgeUpdate Pause()
    {
        PauseCalls++;
        return ExecuteCommand("pause");
    }

    public TimerBridgeUpdate Resume()
    {
        ResumeCalls++;
        return ExecuteCommand("resume");
    }

    public TimerBridgeUpdate Reset()
    {
        ResetCalls++;
        return ExecuteCommand("reset");
    }

    public TimerBridgeUpdate Tick(int elapsedSeconds)
    {
        TickCalls++;
        LastElapsedSeconds = elapsedSeconds;
        return ExecuteCommand("tick");
    }

    private TimerBridgeUpdate ExecuteCommand(string modelName)
    {
        if (ThrowOnNextCommand)
        {
            ThrowOnNextCommand = false;
            throw new InvalidOperationException("Technical test exception");
        }

        CurrentModel = TestModelFactory.Create($"{modelName}-{++modelSequence}");
        var actions = ActionsByCommand.Count > 0
            ? ActionsByCommand.Dequeue()
            : Array.Empty<SystemActionRequest>();

        return new TimerBridgeUpdate(CurrentModel, actions);
    }
}

internal sealed class FakeAudioServicePort : IAudioServicePort
{
    public List<string> Calls { get; } = new();
    public AudioServiceResult StartResult { get; set; } = TestAudioResults.Success;
    public AudioServiceResult StopResult { get; set; } = TestAudioResults.Success;
    public bool ThrowOnStart { get; set; }
    public bool ThrowOnStop { get; set; }

    public AudioServiceResult StartFinalAlertSound()
    {
        Calls.Add("start");
        if (ThrowOnStart)
        {
            throw new InvalidOperationException("Technical test exception");
        }

        return StartResult;
    }

    public AudioServiceResult StopFinalAlertSound()
    {
        Calls.Add("stop");
        if (ThrowOnStop)
        {
            throw new InvalidOperationException("Technical test exception");
        }

        return StopResult;
    }
}

internal static class TestModelFactory
{
    public static TimerDisplayModel Create(string value)
    {
        return new TimerDisplayModel(
            value,
            value,
            value,
            value,
            CanStart: true,
            CanPause: true,
            CanResume: true,
            CanReset: true,
            IsConfigured: true,
            IsFinalAlertActive: false,
            ErrorMessageText: string.Empty,
            EventMessageText: string.Empty,
            AccessibleStatusText: value,
            AccessibleEventText: string.Empty);
    }
}

internal static class TestAudioResults
{
    public static AudioServiceResult Success { get; } = new(
        AudioActionResult.Success,
        AudioActionResult.NotAttempted,
        AudioActionResult.NotAttempted);

    public static AudioServiceResult PartialFocusUnavailable { get; } = new(
        AudioActionResult.Success,
        AudioActionResult.AudioFocusUnavailable,
        AudioActionResult.NotAttempted);

    public static AudioServiceResult PlaybackFailed { get; } = new(
        AudioActionResult.PlaybackFailed,
        AudioActionResult.NotAttempted,
        AudioActionResult.NotAttempted);
}

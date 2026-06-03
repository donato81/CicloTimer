using CicloTimer.App;

namespace CicloTimer.App.Tests;

public sealed class TimerAppOrchestratorDisposeTests
{
    [Fact]
    public void DisposeAttemptsAudioStop()
    {
        var audio = new FakeAudioServicePort();
        var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Dispose();

        Assert.Equal(new[] { "stop" }, audio.Calls);
    }

    [Fact]
    public void DisposeDoesNotCallBridgeCommands()
    {
        var bridge = new FakeTimerBridgePort();
        var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        orchestrator.Dispose();

        Assert.Equal(0, bridge.ConfigureCalls + bridge.StartCalls + bridge.PauseCalls + bridge.ResumeCalls + bridge.ResetCalls + bridge.TickCalls);
    }

    [Fact]
    public void AudioErrorDuringDisposeDoesNotEscape()
    {
        var audio = new FakeAudioServicePort { ThrowOnStop = true };
        var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        var exception = Record.Exception(orchestrator.Dispose);

        Assert.Null(exception);
    }

    [Fact]
    public void AudioErrorDuringDisposeIsNotRethrown()
    {
        var audio = new FakeAudioServicePort { ThrowOnStop = true };
        var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Dispose();

        Assert.Single(audio.Calls);
    }

    [Fact]
    public void DisposeCanBeCalledMultipleTimes()
    {
        var audio = new FakeAudioServicePort();
        var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Dispose();
        var exception = Record.Exception(orchestrator.Dispose);

        Assert.Null(exception);
    }

    [Fact]
    public void DisposeDoesNotCreateRealTimer()
    {
        var fields = typeof(TimerAppOrchestrator).GetFields(
            System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Public);

        Assert.DoesNotContain(fields, field =>
            field.FieldType.FullName is "System.Timers.Timer" or "System.Threading.Timer");
    }

    [Fact]
    public void DisposeDoesNotModifyUi()
    {
        var members = typeof(TimerAppOrchestrator).GetMembers();

        Assert.DoesNotContain(members, member => member.Name.Contains("Window", StringComparison.Ordinal));
    }

    [Fact]
    public void DisposePreservesSafeState()
    {
        var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), new FakeAudioServicePort());
        var before = orchestrator.CurrentState.CurrentModel;

        orchestrator.Dispose();

        Assert.Same(before, orchestrator.CurrentState.CurrentModel);
    }

    [Fact]
    public void DisposeCapturesAudioExceptionsAsTechnicalResult()
    {
        var audio = new FakeAudioServicePort { ThrowOnStop = true };
        var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Dispose();

        Assert.NotNull(orchestrator.CurrentState.LastAudioResult);
    }
}

using CicloTimer.App;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests;

public sealed class TimerAppOrchestratorSafetyTests
{
    [Fact]
    public void RepeatedCommandsDoNotCrash()
    {
        using var orchestrator = new TimerAppOrchestrator(
            new FakeTimerBridgePort(),
            new FakeAudioServicePort());

        var exception = Record.Exception(() =>
        {
            orchestrator.Start();
            orchestrator.Start();
            orchestrator.Pause();
            orchestrator.Resume();
            orchestrator.Reset();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void RepeatedTicksDoNotCrash()
    {
        using var orchestrator = new TimerAppOrchestrator(
            new FakeTimerBridgePort(),
            new FakeAudioServicePort());

        var exception = Record.Exception(() =>
        {
            orchestrator.Tick(1);
            orchestrator.Tick(1);
            orchestrator.Tick(5);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void CloseCommandsKeepCurrentStateCoherent()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        var result = orchestrator.Start();
        var tickResult = orchestrator.Tick(1);

        Assert.Same(tickResult.CurrentModel, orchestrator.CurrentState.CurrentModel);
        Assert.NotSame(result.CurrentModel, orchestrator.CurrentState.CurrentModel);
    }

    [Fact]
    public void AudioActionsAreNotDuplicatedWhenBridgeDoesNotProduceThem()
    {
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Start();
        orchestrator.Tick(1);
        orchestrator.Reset();

        Assert.Empty(audio.Calls);
    }

    [Fact]
    public void BridgeExceptionIsHandledAsTechnicalCommandFailure()
    {
        var bridge = new FakeTimerBridgePort { ThrowOnNextCommand = true };
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        var result = orchestrator.Start();

        Assert.False(result.Success);
        Assert.True(result.HasTechnicalError);
    }

    [Fact]
    public void AudioExceptionIsHandledAsNonBlocking()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort { ThrowOnStart = true };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Start();

        Assert.True(result.Success);
        Assert.True(result.HasTechnicalError);
    }

    [Fact]
    public void NoRealTimerIsCreated()
    {
        var fields = typeof(TimerAppOrchestrator).GetFields(
            System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Public);

        Assert.DoesNotContain(fields, field =>
            field.FieldType.FullName is "System.Timers.Timer" or "System.Threading.Timer");
    }

    [Fact]
    public void PrivateLockFieldSerializesCommands()
    {
        var field = typeof(TimerAppOrchestrator).GetField(
            "_sync",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        Assert.NotNull(field);
        Assert.Equal(typeof(object), field!.FieldType);
    }
}

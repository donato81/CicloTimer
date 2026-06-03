using CicloTimer.App;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests;

public sealed class TimerAppOrchestratorAudioResultTests
{
    [Fact]
    public void AudioResultIsStoredInLastAudioResult()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort { StartResult = TestAudioResults.PartialFocusUnavailable };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Start();

        Assert.Equal(TestAudioResults.PartialFocusUnavailable, result.LastAudioResult);
        Assert.Equal(TestAudioResults.PartialFocusUnavailable, orchestrator.CurrentState.LastAudioResult);
    }

    [Fact]
    public void AudioFocusUnavailableWithPlaybackSuccessDoesNotFailCommand()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort { StartResult = TestAudioResults.PartialFocusUnavailable };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Resume();

        Assert.True(result.Success);
        Assert.True(result.HasAudioWarning);
        Assert.False(result.HasTechnicalError);
    }

    [Fact]
    public void PlaybackFailedDoesNotBlockTimerCommand()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort { StartResult = TestAudioResults.PlaybackFailed };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Tick(1);

        Assert.True(result.Success);
        Assert.True(result.HasTechnicalError);
    }

    [Fact]
    public void AudioExceptionDoesNotEscapeCommand()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort { ThrowOnStart = true };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var exception = Record.Exception(() => orchestrator.Start());

        Assert.Null(exception);
    }

    [Fact]
    public void PartialAudioSuccessIsDistinguishable()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort { StartResult = TestAudioResults.PartialFocusUnavailable };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Start();

        Assert.True(result.HasAudioWarning);
        Assert.Equal(TestAudioResults.PartialFocusUnavailable, result.LastAudioResult);
    }

    [Fact]
    public void AppCommandResultContainsCurrentModel()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        var result = orchestrator.Start();

        Assert.Same(bridge.CurrentModel, result.CurrentModel);
    }

    [Fact]
    public void AppCommandResultContainsLastAudioResult()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StopFinalAlertSound });
        var audio = new FakeAudioServicePort { StopResult = TestAudioResults.Success };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Reset();

        Assert.Equal(TestAudioResults.Success, result.LastAudioResult);
    }

    [Fact]
    public void SuccessRespectsAudioNonBlockingPrinciple()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort { StartResult = TestAudioResults.PlaybackFailed };
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Start();

        Assert.True(result.Success);
    }
}

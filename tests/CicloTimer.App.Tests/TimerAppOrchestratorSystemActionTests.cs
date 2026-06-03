using CicloTimer.App;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests;

public sealed class TimerAppOrchestratorSystemActionTests
{
    [Fact]
    public void ZeroActionsDoNotCallAudio()
    {
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Start();

        Assert.Empty(audio.Calls);
    }

    [Fact]
    public void StartFinalAlertSoundCallsAudioStart()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StartFinalAlertSound });
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        orchestrator.Tick(1);

        Assert.Equal(new[] { "start" }, audio.Calls);
    }

    [Fact]
    public void StopFinalAlertSoundCallsAudioStop()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { SystemActionRequest.StopFinalAlertSound });
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        orchestrator.Pause();

        Assert.Equal(new[] { "stop" }, audio.Calls);
    }

    [Fact]
    public void MultipleActionsAreExecutedInOrder()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[]
        {
            SystemActionRequest.StopFinalAlertSound,
            SystemActionRequest.StartFinalAlertSound
        });
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        orchestrator.Reset();

        Assert.Equal(new[] { "stop", "start" }, audio.Calls);
    }

    [Fact]
    public void UnknownActionIsHandledSafely()
    {
        var bridge = new FakeTimerBridgePort();
        bridge.ActionsByCommand.Enqueue(new[] { (SystemActionRequest)999 });
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        var result = orchestrator.Start();

        Assert.Empty(audio.Calls);
        Assert.Equal(1, result.UnhandledActionCount);
        Assert.True(result.HasAudioWarning);
    }

    [Fact]
    public void PauseDoesNotStopAudioWithoutBridgeAction()
    {
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Pause();

        Assert.Empty(audio.Calls);
    }

    [Fact]
    public void ResetDoesNotStopAudioWithoutBridgeAction()
    {
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Reset();

        Assert.Empty(audio.Calls);
    }

    [Fact]
    public void ResumeDoesNotStartAudioWithoutBridgeAction()
    {
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(new FakeTimerBridgePort(), audio);

        orchestrator.Resume();

        Assert.Empty(audio.Calls);
    }

    [Fact]
    public void OrchestratorDoesNotDeduceAudioFromStates()
    {
        var bridge = new FakeTimerBridgePort
        {
            CurrentModel = new TimerDisplayModel(
                "00:10",
                "final-alert",
                "sessions",
                "pause",
                true,
                true,
                false,
                true,
                true,
                IsFinalAlertActive: true,
                string.Empty,
                string.Empty,
                "status",
                string.Empty)
        };
        var audio = new FakeAudioServicePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, audio);

        orchestrator.Pause();

        Assert.Empty(audio.Calls);
    }

    [Fact]
    public void ActionsAreNotInterpretedThroughFreeStrings()
    {
        var dispatchMethod = typeof(SystemActionDispatcher).GetMethod(nameof(SystemActionDispatcher.Dispatch));

        Assert.NotNull(dispatchMethod);
        Assert.Equal(typeof(IReadOnlyList<SystemActionRequest>), dispatchMethod!.GetParameters()[0].ParameterType);
    }
}

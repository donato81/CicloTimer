using CicloTimer.App;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests;

public sealed class TimerAppOrchestratorStateChangedTests
{
    [Fact]
    public void ConfigureRaisesStateChangedWithUpdatedState()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());
        TimerAppStateChangedEventArgs? observed = null;
        orchestrator.StateChanged += (_, args) => observed = args;

        var result = orchestrator.Configure(new TimerInput(1, 0, 0, 10));

        Assert.NotNull(observed);
        Assert.Same(result, observed!.Result);
        Assert.Same(orchestrator.CurrentState, observed.State);
        Assert.Same(result.CurrentModel, observed.State.CurrentModel);
    }

    [Fact]
    public void StartPauseResumeAndResetRaiseStateChanged()
    {
        using var orchestrator = new TimerAppOrchestrator(
            new FakeTimerBridgePort(),
            new FakeAudioServicePort());
        var observedResults = new List<AppCommandResult>();
        orchestrator.StateChanged += (_, args) => observedResults.Add(args.Result);

        var start = orchestrator.Start();
        var pause = orchestrator.Pause();
        var resume = orchestrator.Resume();
        var reset = orchestrator.Reset();

        Assert.Equal(new[] { start, pause, resume, reset }, observedResults);
    }

    [Fact]
    public void TickRaisesStateChangedAfterDisplayModelUpdate()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());
        TimerAppStateChangedEventArgs? observed = null;
        orchestrator.StateChanged += (_, args) => observed = args;

        var result = orchestrator.Tick(1);

        Assert.NotNull(observed);
        Assert.Same(result, observed!.Result);
        Assert.Same(result.CurrentModel, observed.State.CurrentModel);
        Assert.Equal("tick-1", observed.State.CurrentModel.RemainingTimeText);
        Assert.Same(orchestrator.CurrentState, observed.State);
    }

    [Fact]
    public void StateChangedPayloadMatchesCurrentState()
    {
        using var orchestrator = new TimerAppOrchestrator(
            new FakeTimerBridgePort(),
            new FakeAudioServicePort());
        TimerAppState? observedState = null;
        AppCommandResult? observedResult = null;
        orchestrator.StateChanged += (_, args) =>
        {
            observedState = args.State;
            observedResult = args.Result;
        };

        var result = orchestrator.Start();

        Assert.Same(result, observedResult);
        Assert.Same(orchestrator.CurrentState, observedState);
        Assert.Same(orchestrator.CurrentState.LastCommandResult, observedResult);
    }

    [Fact]
    public void DisposedOrchestratorDoesNotRaiseStateChangedForRejectedCommand()
    {
        var orchestrator = new TimerAppOrchestrator(
            new FakeTimerBridgePort(),
            new FakeAudioServicePort());
        var notificationCount = 0;
        orchestrator.StateChanged += (_, _) => notificationCount++;

        orchestrator.Dispose();
        var result = orchestrator.Start();

        Assert.False(result.Success);
        Assert.Equal(0, notificationCount);
    }
}

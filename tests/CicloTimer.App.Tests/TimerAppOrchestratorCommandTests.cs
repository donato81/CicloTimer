using CicloTimer.App;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests;

public sealed class TimerAppOrchestratorCommandTests
{
    [Fact]
    public void ConfigureForwardsInputToBridge()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());
        var input = new TimerInput(0, 0, -1, 99);

        var result = orchestrator.Configure(input);

        Assert.Equal(1, bridge.ConfigureCalls);
        Assert.Same(input, bridge.LastInput);
        Assert.NotNull(result);
    }

    [Fact]
    public void StartForwardsToBridge()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        orchestrator.Start();

        Assert.Equal(1, bridge.StartCalls);
    }

    [Fact]
    public void PauseForwardsToBridge()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        orchestrator.Pause();

        Assert.Equal(1, bridge.PauseCalls);
    }

    [Fact]
    public void ResumeForwardsToBridge()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        orchestrator.Resume();

        Assert.Equal(1, bridge.ResumeCalls);
    }

    [Fact]
    public void ResetForwardsToBridge()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        orchestrator.Reset();

        Assert.Equal(1, bridge.ResetCalls);
    }

    [Fact]
    public void TickForwardsElapsedSecondsUsingBridgeIntType()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        orchestrator.Tick(7);

        Assert.Equal(1, bridge.TickCalls);
        Assert.Equal(7, bridge.LastElapsedSeconds);
        Assert.Equal(typeof(int), typeof(ITimerBridgePort).GetMethod(nameof(ITimerBridgePort.Tick))!.GetParameters()[0].ParameterType);
    }

    [Fact]
    public void EachCommandUpdatesCurrentState()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        var result = orchestrator.Start();

        Assert.Same(result.CurrentModel, orchestrator.CurrentState.CurrentModel);
        Assert.Same(result, orchestrator.CurrentState.LastCommandResult);
    }

    [Fact]
    public void EachCommandReturnsAppCommandResult()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        var result = orchestrator.Reset();

        Assert.IsType<AppCommandResult>(result);
        Assert.True(result.Success);
    }

    [Fact]
    public void OrchestratorDoesNotValidateDurationsDirectly()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        orchestrator.Configure(new TimerInput(-10, -20, -30, -40));

        Assert.Equal(1, bridge.ConfigureCalls);
    }

    [Fact]
    public void OrchestratorDoesNotGenerateUserTexts()
    {
        var bridge = new FakeTimerBridgePort();
        using var orchestrator = new TimerAppOrchestrator(bridge, new FakeAudioServicePort());

        var result = orchestrator.Start();

        Assert.Same(bridge.CurrentModel, result.CurrentModel);
    }
}

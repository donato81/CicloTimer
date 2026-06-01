using CicloTimer.Core.Timer;

namespace CicloTimer.Core.Tests;

public class TickTests
{
    private static TimerEngine RunningEngine(int session = 300, int alert = 20)
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = session,
            FinalAlertDurationSeconds = alert
        });
        engine.StartTimer();
        return engine;
    }

    [Fact]
    public void Tick_ZeroDuration_Fails()
    {
        var engine = RunningEngine();
        var result = engine.Tick(0);

        Assert.False(result.Success);
        Assert.Equal([TimerError.InvalidTickDuration], result.Errors);
        Assert.Equal(300, engine.RemainingSeconds);
    }

    [Fact]
    public void Tick_NegativeDuration_Fails()
    {
        var engine = RunningEngine();
        var result = engine.Tick(-1);

        Assert.False(result.Success);
        Assert.Equal([TimerError.InvalidTickDuration], result.Errors);
    }

    [Fact]
    public void Tick_FromStopped_HasNoSideEffects()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 20
        });

        var result = engine.Tick(1);

        Assert.True(result.Success);
        Assert.Equal(TimerState.Stopped, engine.CurrentState);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Empty(result.Events);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Tick_FromPaused_HasNoSideEffects()
    {
        var engine = RunningEngine();
        engine.PauseTimer();
        var result = engine.Tick(1);

        Assert.True(result.Success);
        Assert.Equal(TimerState.Paused, engine.CurrentState);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Empty(result.Events);
    }

    [Fact]
    public void Tick_FromRunning_ReducesRemainingSeconds()
    {
        var engine = RunningEngine();
        var result = engine.Tick(1);

        Assert.True(result.Success);
        Assert.Equal(299, engine.RemainingSeconds);
        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Empty(result.Events);
    }

    [Fact]
    public void Tick_ExceedingRemainingTime_CompletesSessionAndDiscardsExcess()
    {
        var engine = RunningEngine(session: 300, alert: 0);
        engine.Tick(297);
        var result = engine.Tick(5);

        Assert.True(result.Success);
        Assert.Equal(1, engine.CompletedSessions);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Equal(
            [TimerEvent.SessionCompleted, TimerEvent.SessionCounterIncremented, TimerEvent.NextSessionStarted],
            result.Events);
    }

    [Fact]
    public void Tick_ExcessNotTransferredToNewSession()
    {
        var engine = RunningEngine(session: 300, alert: 0);
        engine.Tick(297);
        engine.Tick(5);

        Assert.Equal(300, engine.RemainingSeconds);
        Assert.NotEqual(298, engine.RemainingSeconds);
    }

    [Fact]
    public void Tick_DirectToZero_SkipsFinalAlertStarted()
    {
        var engine = RunningEngine(session: 300, alert: 5);
        engine.Tick(290);
        var result = engine.Tick(10);

        Assert.DoesNotContain(TimerEvent.FinalAlertStarted, result.Events);
        Assert.Equal(
            [TimerEvent.SessionCompleted, TimerEvent.SessionCounterIncremented, TimerEvent.NextSessionStarted],
            result.Events);
    }
}

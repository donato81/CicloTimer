using CicloTimer.Core.Timer;

namespace CicloTimer.Core.Tests;

public class StartPauseResumeResetTests
{
    private static TimerEngine ConfiguredEngine(int session = 300, int alert = 20)
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = session,
            FinalAlertDurationSeconds = alert
        });
        return engine;
    }

    [Fact]
    public void StartTimer_WithoutConfiguration_Fails()
    {
        var engine = new TimerEngine();
        var result = engine.StartTimer();

        Assert.False(result.Success);
        Assert.Equal([TimerError.TimerNotConfigured], result.Errors);
    }

    [Fact]
    public void StartTimer_WithValidConfiguration_Succeeds()
    {
        var engine = ConfiguredEngine();
        var result = engine.StartTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Equal([TimerEvent.TimerStarted], result.Events);
        Assert.False(engine.IsFinalAlertActive);
    }

    [Fact]
    public void StartTimer_FromRunning_Fails()
    {
        var engine = ConfiguredEngine();
        engine.StartTimer();
        var result = engine.StartTimer();

        Assert.False(result.Success);
        Assert.Equal([TimerError.CannotStart], result.Errors);
    }

    [Fact]
    public void StartTimer_DoesNotStartInFinalAlert()
    {
        var engine = ConfiguredEngine();
        engine.StartTimer();

        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.NotEqual(TimerState.FinalAlert, engine.CurrentState);
    }

    [Fact]
    public void PauseTimer_FromRunning_Succeeds()
    {
        var engine = ConfiguredEngine();
        engine.StartTimer();
        var result = engine.PauseTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.Paused, engine.CurrentState);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Equal(0, engine.CompletedSessions);
        Assert.Equal([TimerEvent.TimerPaused], result.Events);
    }

    [Fact]
    public void PauseTimer_FromFinalAlert_Succeeds()
    {
        var engine = ConfiguredEngine();
        engine.StartTimer();
        engine.Tick(281);
        var result = engine.PauseTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.Paused, engine.CurrentState);
        Assert.Equal(19, engine.RemainingSeconds);
    }

    [Fact]
    public void PauseTimer_FromInvalidState_Fails()
    {
        var engine = ConfiguredEngine();
        var result = engine.PauseTimer();

        Assert.False(result.Success);
        Assert.Equal([TimerError.CannotPause], result.Errors);
    }

    [Fact]
    public void ResumeTimer_OutsideFinalAlert_ReturnsRunning()
    {
        var engine = ConfiguredEngine();
        engine.StartTimer();
        engine.PauseTimer();
        var result = engine.ResumeTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Equal([TimerEvent.TimerResumed], result.Events);
        Assert.False(engine.IsFinalAlertActive);
    }

    [Fact]
    public void ResumeTimer_InsideFinalAlert_ReturnsFinalAlertWithOrderedEvents()
    {
        var engine = ConfiguredEngine();
        engine.StartTimer();
        engine.Tick(281);
        engine.PauseTimer();
        var result = engine.ResumeTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.FinalAlert, engine.CurrentState);
        Assert.True(engine.IsFinalAlertActive);
        Assert.Equal([TimerEvent.TimerResumed, TimerEvent.FinalAlertStarted], result.Events);
    }

    [Fact]
    public void ResumeTimer_WithZeroFinalAlert_ReturnsRunning()
    {
        var engine = ConfiguredEngine(alert: 0);
        engine.StartTimer();
        engine.Tick(10);
        engine.PauseTimer();
        var result = engine.ResumeTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Equal([TimerEvent.TimerResumed], result.Events);
    }

    [Fact]
    public void ResumeTimer_FromInvalidState_Fails()
    {
        var engine = ConfiguredEngine();
        var result = engine.ResumeTimer();

        Assert.False(result.Success);
        Assert.Equal([TimerError.CannotResume], result.Errors);
    }

    [Fact]
    public void ResetTimer_FromRunning_Succeeds()
    {
        var engine = ConfiguredEngine();
        engine.StartTimer();
        engine.Tick(100);
        var result = engine.ResetTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.Stopped, engine.CurrentState);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Equal(0, engine.CompletedSessions);
        Assert.Equal([TimerEvent.TimerReset], result.Events);
    }

    [Fact]
    public void ResetTimer_FromStopped_ProducesTimerReset()
    {
        var engine = ConfiguredEngine();
        var result = engine.ResetTimer();

        Assert.True(result.Success);
        Assert.Equal(TimerState.Stopped, engine.CurrentState);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Equal([TimerEvent.TimerReset], result.Events);
        Assert.DoesNotContain(TimerEvent.SessionCompleted, result.Events);
        Assert.DoesNotContain(TimerEvent.SessionCounterIncremented, result.Events);
        Assert.DoesNotContain(TimerEvent.NextSessionStarted, result.Events);
    }

    [Fact]
    public void ResetTimer_WithoutConfiguration_Fails()
    {
        var engine = new TimerEngine();
        var result = engine.ResetTimer();

        Assert.False(result.Success);
        Assert.Equal([TimerError.TimerNotConfigured], result.Errors);
    }

    [Fact]
    public void ResetTimer_DoesNotAlterCompletedSessions()
    {
        var engine = ConfiguredEngine(session: 10, alert: 0);
        engine.StartTimer();
        engine.Tick(10);
        Assert.Equal(1, engine.CompletedSessions);

        engine.ResetTimer();
        Assert.Equal(1, engine.CompletedSessions);
    }
}

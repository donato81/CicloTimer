using CicloTimer.Core.Timer;

namespace CicloTimer.Core.Tests;

public class SessionCompletionTests
{
    [Fact]
    public void SessionCompletion_IncrementsCounterAndRestartsAutomatically()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 10,
            FinalAlertDurationSeconds = 0
        });
        engine.StartTimer();
        var result = engine.Tick(10);

        Assert.Equal(1, engine.CompletedSessions);
        Assert.Equal(10, engine.RemainingSeconds);
        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Equal(
            [TimerEvent.SessionCompleted, TimerEvent.SessionCounterIncremented, TimerEvent.NextSessionStarted],
            result.Events);
    }

    [Fact]
    public void SessionCompletion_EventOrderIsPreserved()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 5,
            FinalAlertDurationSeconds = 0
        });
        engine.StartTimer();
        var result = engine.Tick(5);

        Assert.Equal(TimerEvent.SessionCompleted, result.Events[0]);
        Assert.Equal(TimerEvent.SessionCounterIncremented, result.Events[1]);
        Assert.Equal(TimerEvent.NextSessionStarted, result.Events[2]);
    }

    [Fact]
    public void MultipleSessions_IncrementCounterCorrectly()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 5,
            FinalAlertDurationSeconds = 0
        });
        engine.StartTimer();
        engine.Tick(5);
        engine.Tick(5);

        Assert.Equal(2, engine.CompletedSessions);
    }

    [Fact]
    public void Reset_DoesNotIncrementCounter()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 100,
            FinalAlertDurationSeconds = 0
        });
        engine.StartTimer();
        engine.Tick(50);
        engine.ResetTimer();

        Assert.Equal(0, engine.CompletedSessions);
    }

    [Fact]
    public void Pause_DoesNotCompleteSession()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 10,
            FinalAlertDurationSeconds = 0
        });
        engine.StartTimer();
        engine.Tick(5);
        engine.PauseTimer();

        Assert.Equal(0, engine.CompletedSessions);
        Assert.Equal(TimerState.Paused, engine.CurrentState);
    }

    [Fact]
    public void AfterAutoRestart_StateIsImmediatelyRunning()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 3,
            FinalAlertDurationSeconds = 1
        });
        engine.StartTimer();
        engine.Tick(3);

        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Equal(3, engine.RemainingSeconds);
        Assert.True(engine.CanPause);
        Assert.True(engine.CanReset);
        Assert.False(engine.IsFinalAlertActive);
    }
}

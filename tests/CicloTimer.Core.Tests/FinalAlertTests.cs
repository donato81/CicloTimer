using CicloTimer.Core.Timer;

namespace CicloTimer.Core.Tests;

public class FinalAlertTests
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
    public void Tick_EnteringFinalAlert_ProducesFinalAlertStartedOnce()
    {
        var engine = RunningEngine();
        engine.Tick(279);
        var result = engine.Tick(1);

        Assert.Equal(TimerState.FinalAlert, engine.CurrentState);
        Assert.True(engine.IsFinalAlertActive);
        Assert.Equal([TimerEvent.FinalAlertStarted], result.Events);
    }

    [Fact]
    public void Tick_SubsequentTicksInFinalAlert_DoNotRepeatFinalAlertStarted()
    {
        var engine = RunningEngine();
        engine.Tick(281);
        var secondTick = engine.Tick(1);

        Assert.Empty(secondTick.Events);
        Assert.Equal(TimerState.FinalAlert, engine.CurrentState);
    }

    [Fact]
    public void ZeroFinalAlert_NeverEntersFinalAlertState()
    {
        var engine = RunningEngine(alert: 0);
        engine.Tick(299);
        var result = engine.Tick(1);

        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.False(engine.IsFinalAlertActive);
        Assert.DoesNotContain(TimerEvent.FinalAlertStarted, result.Events);
    }

    [Fact]
    public void IsFinalAlertActive_IsTrueOnlyInFinalAlertState()
    {
        var engine = RunningEngine();
        Assert.False(engine.IsFinalAlertActive);

        engine.Tick(281);
        Assert.True(engine.IsFinalAlertActive);

        engine.PauseTimer();
        Assert.False(engine.IsFinalAlertActive);

        engine.ResumeTimer();
        Assert.True(engine.IsFinalAlertActive);
    }

    [Fact]
    public void PauseAndResumeInFinalAlert_ProducesFinalAlertStartedOnResume()
    {
        var engine = RunningEngine(session: 100, alert: 20);
        engine.Tick(81);
        engine.PauseTimer();
        var result = engine.ResumeTimer();

        Assert.Equal(TimerState.FinalAlert, engine.CurrentState);
        Assert.Equal([TimerEvent.TimerResumed, TimerEvent.FinalAlertStarted], result.Events);
    }
}

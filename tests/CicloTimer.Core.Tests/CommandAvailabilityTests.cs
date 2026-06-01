using CicloTimer.Core.Timer;

namespace CicloTimer.Core.Tests;

public class CommandAvailabilityTests
{
    [Fact]
    public void CanStart_OnlyWhenConfiguredAndStopped()
    {
        var engine = new TimerEngine();
        Assert.False(engine.CanStart);

        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 20
        });
        Assert.True(engine.CanStart);

        engine.StartTimer();
        Assert.False(engine.CanStart);
    }

    [Fact]
    public void CanPause_OnlyWhenRunningOrFinalAlert()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 20
        });

        Assert.False(engine.CanPause);

        engine.StartTimer();
        Assert.True(engine.CanPause);

        engine.Tick(281);
        Assert.True(engine.CanPause);

        engine.PauseTimer();
        Assert.False(engine.CanPause);
    }

    [Fact]
    public void CanResume_OnlyWhenPaused()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 20
        });
        engine.StartTimer();

        Assert.False(engine.CanResume);

        engine.PauseTimer();
        Assert.True(engine.CanResume);

        engine.ResumeTimer();
        Assert.False(engine.CanResume);
    }

    [Fact]
    public void CanReset_OnlyWhenConfigured()
    {
        var engine = new TimerEngine();
        Assert.False(engine.CanReset);

        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 20
        });
        Assert.True(engine.CanReset);

        engine.StartTimer();
        Assert.True(engine.CanReset);

        engine.PauseTimer();
        Assert.True(engine.CanReset);
    }

    [Fact]
    public void CanProperties_AreNotInCommandResult()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 20
        });
        var result = engine.StartTimer();

        Assert.True(engine.CanPause);
        Assert.True(engine.CanReset);
        Assert.False(engine.CanStart);
        Assert.False(engine.CanResume);

        var resultType = typeof(TimerCommandResult);
        Assert.Null(resultType.GetProperty("CanStart"));
        Assert.Null(resultType.GetProperty("CanPause"));
        Assert.Null(resultType.GetProperty("CanResume"));
        Assert.Null(resultType.GetProperty("CanReset"));
    }
}

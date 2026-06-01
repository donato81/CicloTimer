using CicloTimer.Core.Timer;

namespace CicloTimer.Core.Tests;

public class ConfigurationTests
{
    private static TimerConfiguration ValidConfig() => new()
    {
        SessionDurationSeconds = 300,
        FinalAlertDurationSeconds = 20
    };

    [Fact]
    public void InitialState_IsNotConfigured()
    {
        var engine = new TimerEngine();

        Assert.False(engine.IsConfigured);
        Assert.Equal(TimerState.Stopped, engine.CurrentState);
        Assert.Equal(0, engine.CompletedSessions);
        Assert.Equal(0, engine.RemainingSeconds);
    }

    [Fact]
    public void ConfigureTimer_ValidConfiguration_Succeeds()
    {
        var engine = new TimerEngine();
        var config = ValidConfig();

        var result = engine.ConfigureTimer(config);

        Assert.True(result.Success);
        Assert.True(engine.IsConfigured);
        Assert.True(result.IsConfigured);
        Assert.Equal(TimerState.Stopped, engine.CurrentState);
        Assert.Equal(300, engine.RemainingSeconds);
        Assert.Equal([TimerEvent.TimerConfigured], result.Events);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ConfigureTimer_ZeroSessionDuration_Fails()
    {
        var engine = new TimerEngine();
        var result = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 0,
            FinalAlertDurationSeconds = 10
        });

        Assert.False(result.Success);
        Assert.False(engine.IsConfigured);
        Assert.Equal([TimerEvent.ValidationFailed], result.Events);
        Assert.Equal([TimerError.InvalidSessionDuration], result.Errors);
    }

    [Fact]
    public void ConfigureTimer_NegativeFinalAlert_Fails()
    {
        var engine = new TimerEngine();
        var result = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = -1
        });

        Assert.False(result.Success);
        Assert.Equal([TimerError.InvalidFinalAlertDuration], result.Errors);
        Assert.Equal([TimerEvent.ValidationFailed], result.Events);
    }

    [Fact]
    public void ConfigureTimer_FinalAlertEqualToSession_Fails()
    {
        var engine = new TimerEngine();
        var result = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 300
        });

        Assert.False(result.Success);
        Assert.Equal([TimerError.FinalAlertNotLessThanSessionDuration], result.Errors);
    }

    [Fact]
    public void ConfigureTimer_FinalAlertGreaterThanSession_Fails()
    {
        var engine = new TimerEngine();
        var result = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 301
        });

        Assert.False(result.Success);
        Assert.Equal([TimerError.FinalAlertNotLessThanSessionDuration], result.Errors);
    }

    [Fact]
    public void ConfigureTimer_InvalidAfterValid_KeepsPreviousConfiguration()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(ValidConfig());
        engine.StartTimer();
        engine.Tick(50);

        var result = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 0,
            FinalAlertDurationSeconds = 10
        });

        Assert.False(result.Success);
        Assert.True(engine.IsConfigured);
        Assert.Equal(TimerState.Running, engine.CurrentState);
        Assert.Equal(250, engine.RemainingSeconds);
    }

    [Fact]
    public void ConfigureTimer_ZeroFinalAlert_IsValid()
    {
        var engine = new TimerEngine();
        var result = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 0
        });

        Assert.True(result.Success);
        Assert.True(engine.IsConfigured);
        Assert.Equal([TimerEvent.TimerConfigured], result.Events);
    }
}

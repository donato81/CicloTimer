using CicloTimer.Core.Timer;

namespace CicloTimer.Core.Tests;

public class EventsAndResultTests
{
    [Fact]
    public void EventsList_IsNotCumulativeAcrossCommands()
    {
        var engine = new TimerEngine();
        var configureResult = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 300,
            FinalAlertDurationSeconds = 20
        });
        var startResult = engine.StartTimer();
        var tickResult = engine.Tick(1);

        Assert.Equal([TimerEvent.TimerConfigured], configureResult.Events);
        Assert.Equal([TimerEvent.TimerStarted], startResult.Events);
        Assert.Empty(tickResult.Events);
        Assert.DoesNotContain(TimerEvent.TimerConfigured, startResult.Events);
        Assert.DoesNotContain(TimerEvent.TimerStarted, tickResult.Events);
    }

    [Fact]
    public void CommandResult_IsSnapshotOfCurrentCommand()
    {
        var engine = new TimerEngine();
        engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 100,
            FinalAlertDurationSeconds = 10
        });
        engine.StartTimer();
        engine.Tick(30);
        var result = engine.PauseTimer();

        Assert.Equal(TimerState.Paused, result.State);
        Assert.Equal(70, result.RemainingSeconds);
        Assert.Equal(TimerState.Paused, engine.CurrentState);
        Assert.Equal(70, engine.RemainingSeconds);
    }

    [Fact]
    public void ValidationFailed_IsEventNotError()
    {
        var engine = new TimerEngine();
        var result = engine.ConfigureTimer(new TimerConfiguration
        {
            SessionDurationSeconds = 0,
            FinalAlertDurationSeconds = 10
        });

        Assert.Contains(TimerEvent.ValidationFailed, result.Events);
        Assert.Contains(TimerError.InvalidSessionDuration, result.Errors);
        Assert.DoesNotContain(result.Events, e => e == TimerEvent.TimerConfigured);
    }

    [Fact]
    public void MainTypes_ArePublic()
    {
        Assert.True(typeof(TimerState).IsPublic);
        Assert.True(typeof(TimerError).IsPublic);
        Assert.True(typeof(TimerEvent).IsPublic);
        Assert.True(typeof(TimerConfiguration).IsPublic);
        Assert.True(typeof(TimerCommandResult).IsPublic);
        Assert.True(typeof(TimerEngine).IsPublic);
    }

    [Fact]
    public void TimerState_ContainsOnlyApprovedValues()
    {
        var values = Enum.GetNames<TimerState>();
        Assert.Equal(["Stopped", "Running", "FinalAlert", "Paused"], values);
    }
}

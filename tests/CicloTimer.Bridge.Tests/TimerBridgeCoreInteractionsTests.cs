using Xunit;

namespace CicloTimer.Bridge.Tests;

public sealed class TimerBridgeCoreInteractionsTests
{
    [Fact]
    public void Configure_ConvertsMinutesAndSecondsToCoreSeconds()
    {
        var bridge = new TimerBridge();

        var update = bridge.Configure(new TimerInput(5, 0, 0, 20));

        Assert.Equal("05:00", update.DisplayModel.RemainingTimeText);
        Assert.Equal("Timer fermo", update.DisplayModel.TimerStateText);
        Assert.Equal("Sessioni completate: 0", update.DisplayModel.CompletedSessionsText);
        Assert.True(update.DisplayModel.CanStart);
    }

    [Fact]
    public void Configure_InvalidInputPassesThroughCoreValidation()
    {
        var bridge = new TimerBridge();

        var update = bridge.Configure(new TimerInput(0, 0, 0, 20));

        Assert.Equal("La durata della sessione deve essere maggiore di zero.", update.DisplayModel.ErrorMessageText);
        Assert.Equal("Configurazione o comando non valido.", update.DisplayModel.EventMessageText);
        Assert.False(update.DisplayModel.IsConfigured);
    }

    [Fact]
    public void StartPauseResumeReset_ProduceExpectedDisplayStates()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(5, 0, 0, 20));

        var started = bridge.Start();
        var paused = bridge.Pause();
        var resumed = bridge.Resume();
        var reset = bridge.Reset();

        Assert.Equal("Sessione in corso", started.DisplayModel.TimerStateText);
        Assert.Equal("Timer in pausa", paused.DisplayModel.TimerStateText);
        Assert.Equal("Sessione in corso", resumed.DisplayModel.TimerStateText);
        Assert.Equal("Timer fermo", reset.DisplayModel.TimerStateText);
    }

    [Fact]
    public void Reset_DoesNotClearCompletedSessions()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(0, 2, 0, 0));
        bridge.Start();
        bridge.Tick(2);

        var reset = bridge.Reset();

        Assert.Equal("Sessioni completate: 1", reset.DisplayModel.CompletedSessionsText);
        Assert.Equal("Timer fermo", reset.DisplayModel.TimerStateText);
    }

    [Fact]
    public void Tick_ForwardsElapsedSecondsToCoreWithoutCorrection()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(0, 5, 0, 2));
        bridge.Start();

        var update = bridge.Tick(2);

        Assert.Equal("00:03", update.DisplayModel.RemainingTimeText);
        Assert.Empty(update.SystemActions);
    }

    [Fact]
    public void Tick_EnteringFinalAlertProducesStartFinalAlertSound()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(0, 5, 0, 2));
        bridge.Start();

        var update = bridge.Tick(3);

        Assert.Equal("Avviso finale in corso", update.DisplayModel.TimerStateText);
        Assert.Contains(SystemActionRequest.StartFinalAlertSound, update.SystemActions);
    }

    [Fact]
    public void Tick_SessionCompletedFromFinalAlertProducesSyntheticMessageAndStopAction()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(0, 3, 0, 2));
        bridge.Start();
        bridge.Tick(1);

        var update = bridge.Tick(2);

        Assert.Equal("Sessione completata. Sessioni completate: 1.", update.DisplayModel.EventMessageText);
        Assert.Equal(update.DisplayModel.EventMessageText, update.DisplayModel.AccessibleEventText);
        Assert.Equal("Sessioni completate: 1", update.DisplayModel.CompletedSessionsText);
        Assert.Contains(SystemActionRequest.StopFinalAlertSound, update.SystemActions);
    }

    [Fact]
    public void Tick_WithoutRelevantEventsProducesEmptyActionsAndNoAccessibleEvent()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(0, 5, 0, 2));
        bridge.Start();

        var update = bridge.Tick(1);

        Assert.Empty(update.SystemActions);
        Assert.Equal(string.Empty, update.DisplayModel.EventMessageText);
        Assert.Equal(string.Empty, update.DisplayModel.AccessibleEventText);
        Assert.False(string.IsNullOrWhiteSpace(update.DisplayModel.AccessibleStatusText));
    }

    [Fact]
    public void CompletedSessionsText_UsesLocalizedLabelAndCurrentValue()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(0, 1, 0, 0));
        bridge.Start();
        bridge.Tick(1);
        bridge.Tick(1);
        var update = bridge.Tick(1);

        Assert.Equal("Sessioni completate: 3", update.DisplayModel.CompletedSessionsText);
    }
}

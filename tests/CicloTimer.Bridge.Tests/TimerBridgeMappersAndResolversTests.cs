using CicloTimer.Core.Timer;
using Xunit;

namespace CicloTimer.Bridge.Tests;

public sealed class TimerBridgeMappersAndResolversTests
{
    [Theory]
    [InlineData(TimerState.Stopped, "Timer fermo")]
    [InlineData(TimerState.Running, "Sessione in corso")]
    [InlineData(TimerState.FinalAlert, "Avviso finale in corso")]
    [InlineData(TimerState.Paused, "Timer in pausa")]
    public void StateMapper_ReturnsLocalizedText(TimerState state, string expected)
    {
        var mapper = new TimerStateTextMapper();

        Assert.Equal(expected, mapper.Map(state));
    }

    [Theory]
    [InlineData(TimerError.InvalidSessionDuration, "La durata della sessione deve essere maggiore di zero.")]
    [InlineData(TimerError.InvalidFinalAlertDuration, "La durata dell'avviso finale non può essere negativa.")]
    [InlineData(TimerError.FinalAlertNotLessThanSessionDuration, "La durata dell'avviso finale deve essere inferiore alla durata della sessione.")]
    [InlineData(TimerError.TimerNotConfigured, "Configura il timer prima di avviarlo.")]
    [InlineData(TimerError.CannotStart, "Il timer non può essere avviato nello stato corrente.")]
    [InlineData(TimerError.CannotPause, "Il timer non può essere messo in pausa nello stato corrente.")]
    [InlineData(TimerError.CannotResume, "Il timer non può essere ripreso nello stato corrente.")]
    [InlineData(TimerError.CannotReset, "Il timer non può essere resettato nello stato corrente.")]
    [InlineData(TimerError.InvalidTickDuration, "Errore interno: durata tick non valida.")]
    public void ErrorMapper_ReturnsLocalizedText(TimerError error, string expected)
    {
        var mapper = new TimerErrorTextMapper();

        Assert.Equal(expected, mapper.Map([error]));
    }

    [Fact]
    public void ErrorMapper_UsesFirstError()
    {
        var mapper = new TimerErrorTextMapper();

        var text = mapper.Map([TimerError.CannotStart, TimerError.CannotReset]);

        Assert.Equal("Il timer non può essere avviato nello stato corrente.", text);
    }

    [Fact]
    public void ErrorMapper_ReturnsEmptyTextWhenErrorsAreEmpty()
    {
        var mapper = new TimerErrorTextMapper();

        Assert.Equal(string.Empty, mapper.Map([]));
    }

    [Theory]
    [InlineData(TimerEvent.TimerConfigured, "Timer configurato.")]
    [InlineData(TimerEvent.TimerStarted, "Timer avviato.")]
    [InlineData(TimerEvent.TimerPaused, "Timer in pausa.")]
    [InlineData(TimerEvent.TimerResumed, "Timer ripreso.")]
    [InlineData(TimerEvent.TimerReset, "Timer resettato.")]
    [InlineData(TimerEvent.FinalAlertStarted, "Avviso finale iniziato.")]
    [InlineData(TimerEvent.SessionCompleted, "Sessione completata.")]
    [InlineData(TimerEvent.SessionCounterIncremented, "Sessioni completate aggiornate.")]
    [InlineData(TimerEvent.NextSessionStarted, "Nuova sessione avviata.")]
    [InlineData(TimerEvent.ValidationFailed, "Configurazione o comando non valido.")]
    public void EventMapper_ReturnsLocalizedTextForSingleEvent(TimerEvent timerEvent, string expected)
    {
        var mapper = new TimerEventTextMapper();

        Assert.Equal(expected, mapper.Map([timerEvent], completedSessions: 3));
    }

    [Fact]
    public void EventMapper_ReturnsEmptyTextWhenEventsAreEmpty()
    {
        var mapper = new TimerEventTextMapper();

        Assert.Equal(string.Empty, mapper.Map([], completedSessions: 3));
    }

    [Fact]
    public void EventMapper_SynthesizesSessionCompletedMessageForSameResult()
    {
        var mapper = new TimerEventTextMapper();

        var text = mapper.Map(
            [TimerEvent.SessionCompleted, TimerEvent.SessionCounterIncremented, TimerEvent.NextSessionStarted],
            completedSessions: 3);

        Assert.Equal("Sessione completata. Sessioni completate: 3.", text);
    }

    [Fact]
    public void EventMapper_DoesNotSynthesizeEventsFromSeparateResults()
    {
        var mapper = new TimerEventTextMapper();

        var first = mapper.Map([TimerEvent.SessionCompleted], completedSessions: 3);
        var second = mapper.Map([TimerEvent.SessionCounterIncremented, TimerEvent.NextSessionStarted], completedSessions: 3);

        Assert.Equal("Sessione completata.", first);
        Assert.Equal("Sessioni completate aggiornate.", second);
    }

    [Theory]
    [InlineData(TimerState.Stopped, true, false, false, false, "Avvia")]
    [InlineData(TimerState.Running, false, true, false, true, "Pausa")]
    [InlineData(TimerState.FinalAlert, false, true, false, true, "Pausa")]
    [InlineData(TimerState.Paused, false, false, true, true, "Riprendi")]
    [InlineData(TimerState.Stopped, false, false, false, false, "")]
    public void PrimaryActionResolver_ReturnsExpectedText(
        TimerState state,
        bool canStart,
        bool canPause,
        bool canResume,
        bool canReset,
        string expected)
    {
        var resolver = new PrimaryActionResolver();

        Assert.Equal(expected, resolver.Resolve(state, canStart, canPause, canResume, canReset));
    }

    [Fact]
    public void SystemActionResolver_StartsFinalAlertSoundForFinalAlertStarted()
    {
        var resolver = new SystemActionResolver();

        var actions = resolver.Resolve([TimerEvent.FinalAlertStarted], wasFinalAlertActive: false, isFinalAlertActive: true);

        Assert.Contains(SystemActionRequest.StartFinalAlertSound, actions);
    }

    [Fact]
    public void SystemActionResolver_StopsFinalAlertSoundForSessionCompletionDuringFinalAlert()
    {
        var resolver = new SystemActionResolver();

        var actions = resolver.Resolve(
            [TimerEvent.SessionCompleted, TimerEvent.SessionCounterIncremented, TimerEvent.NextSessionStarted],
            wasFinalAlertActive: true,
            isFinalAlertActive: false);

        Assert.Contains(SystemActionRequest.StopFinalAlertSound, actions);
    }

    [Fact]
    public void SystemActionResolver_StopsFinalAlertSoundForPauseDuringFinalAlert()
    {
        var resolver = new SystemActionResolver();

        var actions = resolver.Resolve([TimerEvent.TimerPaused], wasFinalAlertActive: true, isFinalAlertActive: false);

        Assert.Contains(SystemActionRequest.StopFinalAlertSound, actions);
    }

    [Fact]
    public void SystemActionResolver_StopsFinalAlertSoundForResetDuringFinalAlert()
    {
        var resolver = new SystemActionResolver();

        var actions = resolver.Resolve([TimerEvent.TimerReset], wasFinalAlertActive: true, isFinalAlertActive: false);

        Assert.Contains(SystemActionRequest.StopFinalAlertSound, actions);
    }

    [Theory]
    [InlineData(TimerEvent.TimerPaused)]
    [InlineData(TimerEvent.TimerReset)]
    public void SystemActionResolver_DoesNotStopFinalAlertSoundOutsideFinalAlert(TimerEvent timerEvent)
    {
        var resolver = new SystemActionResolver();

        var actions = resolver.Resolve([timerEvent], wasFinalAlertActive: false, isFinalAlertActive: false);

        Assert.DoesNotContain(SystemActionRequest.StopFinalAlertSound, actions);
    }

    [Fact]
    public void SystemActionResolver_ReturnsEmptyNonNullListForNoRelevantEvents()
    {
        var resolver = new SystemActionResolver();

        var actions = resolver.Resolve([], wasFinalAlertActive: false, isFinalAlertActive: false);

        Assert.NotNull(actions);
        Assert.Empty(actions);
    }
}

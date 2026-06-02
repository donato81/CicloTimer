using CicloTimer.Localization.Locales.It;

namespace CicloTimer.Localization.Tests;

public sealed class ItalianTimerTextsTests
{
    [Theory]
    [InlineData(TimerTextKey.StateStopped, "Timer fermo")]
    [InlineData(TimerTextKey.StateRunning, "Sessione in corso")]
    [InlineData(TimerTextKey.StateFinalAlert, "Avviso finale in corso")]
    [InlineData(TimerTextKey.StatePaused, "Timer in pausa")]
    [InlineData(TimerTextKey.EventTimerConfigured, "Timer configurato.")]
    [InlineData(TimerTextKey.EventTimerStarted, "Timer avviato.")]
    [InlineData(TimerTextKey.EventTimerPaused, "Timer in pausa.")]
    [InlineData(TimerTextKey.EventTimerResumed, "Timer ripreso.")]
    [InlineData(TimerTextKey.EventTimerReset, "Timer resettato.")]
    [InlineData(TimerTextKey.EventFinalAlertStarted, "Avviso finale iniziato.")]
    [InlineData(TimerTextKey.EventSessionCompleted, "Sessione completata.")]
    [InlineData(TimerTextKey.EventSessionCounterIncremented, "Sessioni completate aggiornate.")]
    [InlineData(TimerTextKey.EventNextSessionStarted, "Nuova sessione avviata.")]
    [InlineData(TimerTextKey.EventValidationFailed, "Configurazione o comando non valido.")]
    public void GetReturnsExpectedItalianTimerText(TimerTextKey key, string expected)
    {
        Assert.Equal(expected, ItalianTimerTexts.Get(key));
    }
}

namespace CicloTimer.Localization.Locales.It;

public static class ItalianTimerTexts
{
    public static string Get(TimerTextKey key) => key switch
    {
        TimerTextKey.StateStopped => "Timer fermo",
        TimerTextKey.StateRunning => "Sessione in corso",
        TimerTextKey.StateFinalAlert => "Avviso finale in corso",
        TimerTextKey.StatePaused => "Timer in pausa",
        TimerTextKey.EventTimerConfigured => "Timer configurato.",
        TimerTextKey.EventTimerStarted => "Timer avviato.",
        TimerTextKey.EventTimerPaused => "Timer in pausa.",
        TimerTextKey.EventTimerResumed => "Timer ripreso.",
        TimerTextKey.EventTimerReset => "Timer resettato.",
        TimerTextKey.EventFinalAlertStarted => "Avviso finale iniziato.",
        TimerTextKey.EventSessionCompleted => "Sessione completata.",
        TimerTextKey.EventSessionCounterIncremented => "Sessioni completate aggiornate.",
        TimerTextKey.EventNextSessionStarted => "Nuova sessione avviata.",
        TimerTextKey.EventValidationFailed => "Configurazione o comando non valido.",
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, "Timer text key is not supported.")
    };
}

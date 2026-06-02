namespace CicloTimer.Localization.Locales.It;

public static class ItalianErrorTexts
{
    public static string Get(ErrorTextKey key) => key switch
    {
        ErrorTextKey.InvalidSessionDuration => "La durata della sessione deve essere maggiore di zero.",
        ErrorTextKey.InvalidFinalAlertDuration => "La durata dell'avviso finale non può essere negativa.",
        ErrorTextKey.FinalAlertNotLessThanSessionDuration => "La durata dell'avviso finale deve essere inferiore alla durata della sessione.",
        ErrorTextKey.TimerNotConfigured => "Configura il timer prima di avviarlo.",
        ErrorTextKey.CannotStart => "Il timer non può essere avviato nello stato corrente.",
        ErrorTextKey.CannotPause => "Il timer non può essere messo in pausa nello stato corrente.",
        ErrorTextKey.CannotResume => "Il timer non può essere ripreso nello stato corrente.",
        ErrorTextKey.CannotReset => "Il timer non può essere resettato nello stato corrente.",
        ErrorTextKey.InvalidTickDuration => "Errore interno: durata tick non valida.",
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, "Error text key is not supported.")
    };
}

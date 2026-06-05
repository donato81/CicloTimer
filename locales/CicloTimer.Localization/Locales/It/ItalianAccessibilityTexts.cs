namespace CicloTimer.Localization.Locales.It;

public static class ItalianAccessibilityTexts
{
    public static string Get(AccessibilityTextKey key) => key switch
    {
        AccessibilityTextKey.StatusTemplate => "Tempo rimanente: {0}. {1}. {2}.",
        AccessibilityTextKey.SessionCompletedTemplate => "Sessione completata. Sessioni completate: {0}.",
        AccessibilityTextKey.ErrorTemplate => "Errore: {0}",
        AccessibilityTextKey.StartTimer => "Avvia timer",
        AccessibilityTextKey.PauseTimer => "Metti in pausa il timer",
        AccessibilityTextKey.ResumeTimer => "Riprendi timer",
        AccessibilityTextKey.ResetTimer => "Resetta timer",
        AccessibilityTextKey.SessionDurationMinutes => "Durata sessione, minuti",
        AccessibilityTextKey.SessionDurationSeconds => "Durata sessione, secondi",
        AccessibilityTextKey.FinalAlertDurationSeconds => "Durata avviso finale, secondi",
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, "Accessibility text key is not supported.")
    };
}

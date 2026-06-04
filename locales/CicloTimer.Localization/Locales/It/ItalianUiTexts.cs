namespace CicloTimer.Localization.Locales.It;

public static class ItalianUiTexts
{
    public static string Get(UiTextKey key) => key switch
    {
        UiTextKey.AppTitle => "CicloTimer",
        UiTextKey.SessionDuration => "Durata sessione",
        UiTextKey.FinalAlertDuration => "Durata avviso finale",
        UiTextKey.Minutes => "Minuti",
        UiTextKey.Seconds => "Secondi",
        UiTextKey.RemainingTime => "Tempo rimanente",
        UiTextKey.TimerState => "Stato timer",
        UiTextKey.CompletedSessions => "Sessioni completate",
        UiTextKey.Message => "Messaggio",
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, "UI text key is not supported.")
    };
}

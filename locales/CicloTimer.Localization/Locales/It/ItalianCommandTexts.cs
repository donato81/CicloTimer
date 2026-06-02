namespace CicloTimer.Localization.Locales.It;

public static class ItalianCommandTexts
{
    public static string Get(CommandTextKey key) => key switch
    {
        CommandTextKey.Start => "Avvia",
        CommandTextKey.Pause => "Pausa",
        CommandTextKey.Resume => "Riprendi",
        CommandTextKey.Reset => "Reset",
        CommandTextKey.Configure => "Configura",
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, "Command text key is not supported.")
    };
}

using CicloTimer.Localization.Locales.It;

namespace CicloTimer.Localization.Tests;

public sealed class ItalianUiTextsTests
{
    [Theory]
    [InlineData(UiTextKey.AppTitle, "CicloTimer")]
    [InlineData(UiTextKey.SessionDuration, "Durata sessione")]
    [InlineData(UiTextKey.FinalAlertDuration, "Durata avviso finale")]
    [InlineData(UiTextKey.Minutes, "Minuti")]
    [InlineData(UiTextKey.Seconds, "Secondi")]
    [InlineData(UiTextKey.RemainingTime, "Tempo rimanente")]
    [InlineData(UiTextKey.TimerState, "Stato timer")]
    [InlineData(UiTextKey.CompletedSessions, "Sessioni completate")]
    [InlineData(UiTextKey.Message, "Messaggio")]
    public void GetReturnsExpectedItalianUiText(UiTextKey key, string expected)
    {
        Assert.Equal(expected, ItalianUiTexts.Get(key));
    }
}

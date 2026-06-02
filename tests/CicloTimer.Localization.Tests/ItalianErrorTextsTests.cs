using CicloTimer.Localization.Locales.It;

namespace CicloTimer.Localization.Tests;

public sealed class ItalianErrorTextsTests
{
    [Theory]
    [InlineData(ErrorTextKey.InvalidSessionDuration, "La durata della sessione deve essere maggiore di zero.")]
    [InlineData(ErrorTextKey.InvalidFinalAlertDuration, "La durata dell'avviso finale non può essere negativa.")]
    [InlineData(ErrorTextKey.FinalAlertNotLessThanSessionDuration, "La durata dell'avviso finale deve essere inferiore alla durata della sessione.")]
    [InlineData(ErrorTextKey.TimerNotConfigured, "Configura il timer prima di avviarlo.")]
    [InlineData(ErrorTextKey.CannotStart, "Il timer non può essere avviato nello stato corrente.")]
    [InlineData(ErrorTextKey.CannotPause, "Il timer non può essere messo in pausa nello stato corrente.")]
    [InlineData(ErrorTextKey.CannotResume, "Il timer non può essere ripreso nello stato corrente.")]
    [InlineData(ErrorTextKey.CannotReset, "Il timer non può essere resettato nello stato corrente.")]
    [InlineData(ErrorTextKey.InvalidTickDuration, "Errore interno: durata tick non valida.")]
    public void GetReturnsExpectedItalianErrorText(ErrorTextKey key, string expected)
    {
        Assert.Equal(expected, ItalianErrorTexts.Get(key));
    }
}

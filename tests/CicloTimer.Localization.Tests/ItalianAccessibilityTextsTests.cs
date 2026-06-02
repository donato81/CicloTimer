using CicloTimer.Localization.Locales.It;

namespace CicloTimer.Localization.Tests;

public sealed class ItalianAccessibilityTextsTests
{
    [Theory]
    [InlineData(AccessibilityTextKey.StatusTemplate, "Tempo rimanente: {0}. {1}. {2}.")]
    [InlineData(AccessibilityTextKey.SessionCompletedTemplate, "Sessione completata. Sessioni completate: {0}.")]
    [InlineData(AccessibilityTextKey.ErrorTemplate, "Errore: {0}")]
    [InlineData(AccessibilityTextKey.StartTimer, "Avvia timer")]
    [InlineData(AccessibilityTextKey.PauseTimer, "Metti in pausa il timer")]
    [InlineData(AccessibilityTextKey.ResumeTimer, "Riprendi timer")]
    [InlineData(AccessibilityTextKey.ResetTimer, "Resetta timer")]
    public void GetReturnsExpectedItalianAccessibilityText(AccessibilityTextKey key, string expected)
    {
        Assert.Equal(expected, ItalianAccessibilityTexts.Get(key));
    }
}

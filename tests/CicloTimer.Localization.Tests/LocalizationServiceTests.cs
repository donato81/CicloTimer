namespace CicloTimer.Localization.Tests;

public sealed class LocalizationServiceTests
{
    private readonly LocalizationService service = new();

    [Fact]
    public void GetTimerTextReturnsItalianText()
    {
        Assert.Equal("Sessione in corso", service.GetTimerText(TimerTextKey.StateRunning));
    }

    [Fact]
    public void GetCommandTextReturnsItalianText()
    {
        Assert.Equal("Avvia", service.GetCommandText(CommandTextKey.Start));
    }

    [Fact]
    public void GetErrorTextReturnsItalianText()
    {
        Assert.Equal(
            "La durata della sessione deve essere maggiore di zero.",
            service.GetErrorText(ErrorTextKey.InvalidSessionDuration));
    }

    [Fact]
    public void GetAccessibilityTextReturnsItalianText()
    {
        Assert.Equal("Avvia timer", service.GetAccessibilityText(AccessibilityTextKey.StartTimer));
    }

    [Fact]
    public void GetUiTextReturnsItalianText()
    {
        Assert.Equal("Durata sessione", service.GetUiText(UiTextKey.SessionDuration));
    }

    [Fact]
    public void NullLanguageUsesItalian()
    {
        Assert.Equal("Pausa", service.GetCommandText(CommandTextKey.Pause, null));
    }

    [Fact]
    public void ItalianLanguageUsesItalian()
    {
        Assert.Equal("Pausa", service.GetCommandText(CommandTextKey.Pause, SupportedLanguage.It));
    }

    [Fact]
    public void SessionCompletedTemplateFormatsOneParameter()
    {
        var text = service.GetAccessibilityText(
            AccessibilityTextKey.SessionCompletedTemplate,
            SupportedLanguage.It,
            3);

        Assert.Equal("Sessione completata. Sessioni completate: 3.", text);
    }

    [Fact]
    public void ErrorTemplateFormatsOneParameter()
    {
        var text = service.GetAccessibilityText(
            AccessibilityTextKey.ErrorTemplate,
            SupportedLanguage.It,
            "Durata non valida");

        Assert.Equal("Errore: Durata non valida", text);
    }

    [Fact]
    public void StatusTemplateFormatsThreeParameters()
    {
        var text = service.GetAccessibilityText(
            AccessibilityTextKey.StatusTemplate,
            SupportedLanguage.It,
            "04:59",
            "Sessione in corso",
            "Sessioni completate: 3");

        Assert.Equal("Tempo rimanente: 04:59. Sessione in corso. Sessioni completate: 3.", text);
    }

    [Fact]
    public void GetAccessibilityTextWithNullArgsReturnsRawTemplate()
    {
        object[]? args = null;

        var text = service.GetAccessibilityText(
            AccessibilityTextKey.SessionCompletedTemplate,
            SupportedLanguage.It,
            args!);

        Assert.Equal("Sessione completata. Sessioni completate: {0}.", text);
    }

    [Fact]
    public void GetAccessibilityTextWithEmptyArgsReturnsRawTemplate()
    {
        var text = service.GetAccessibilityText(
            AccessibilityTextKey.StatusTemplate,
            SupportedLanguage.It,
            []);

        Assert.Equal("Tempo rimanente: {0}. {1}. {2}.", text);
    }
}

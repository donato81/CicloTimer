using CicloTimer.Localization.Locales.It;

namespace CicloTimer.Localization;

public sealed class LocalizationService
{
    public string GetTimerText(TimerTextKey key, SupportedLanguage? language = null)
    {
        _ = ResolveLanguage(language);
        return ItalianTimerTexts.Get(key);
    }

    public string GetCommandText(CommandTextKey key, SupportedLanguage? language = null)
    {
        _ = ResolveLanguage(language);
        return ItalianCommandTexts.Get(key);
    }

    public string GetErrorText(ErrorTextKey key, SupportedLanguage? language = null)
    {
        _ = ResolveLanguage(language);
        return ItalianErrorTexts.Get(key);
    }

    public string GetAccessibilityText(
        AccessibilityTextKey key,
        SupportedLanguage? language = null,
        params object[] args)
    {
        _ = ResolveLanguage(language);
        var template = ItalianAccessibilityTexts.Get(key);

        return args is null || args.Length == 0
            ? template
            : string.Format(template, args);
    }

    public string GetUiText(UiTextKey key, SupportedLanguage? language = null)
    {
        _ = ResolveLanguage(language);
        return ItalianUiTexts.Get(key);
    }

    private static SupportedLanguage ResolveLanguage(SupportedLanguage? language)
    {
        return language is SupportedLanguage.It or null
            ? SupportedLanguage.It
            : SupportedLanguage.It;
    }
}

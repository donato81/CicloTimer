namespace CicloTimer.Localization.Tests;

public sealed class LocalizationCompletenessTests
{
    private readonly LocalizationService service = new();

    [Fact]
    public void AllTimerTextKeysReturnText()
    {
        foreach (var key in Enum.GetValues<TimerTextKey>())
        {
            AssertValidText(service.GetTimerText(key));
        }
    }

    [Fact]
    public void AllCommandTextKeysReturnText()
    {
        foreach (var key in Enum.GetValues<CommandTextKey>())
        {
            AssertValidText(service.GetCommandText(key));
        }
    }

    [Fact]
    public void AllErrorTextKeysReturnText()
    {
        foreach (var key in Enum.GetValues<ErrorTextKey>())
        {
            AssertValidText(service.GetErrorText(key));
        }
    }

    [Fact]
    public void AllAccessibilityTextKeysReturnText()
    {
        foreach (var key in Enum.GetValues<AccessibilityTextKey>())
        {
            AssertValidText(service.GetAccessibilityText(key));
        }
    }

    [Fact]
    public void AllUiTextKeysReturnText()
    {
        foreach (var key in Enum.GetValues<UiTextKey>())
        {
            AssertValidText(service.GetUiText(key));
        }
    }

    private static void AssertValidText(string text)
    {
        Assert.NotNull(text);
        Assert.NotEmpty(text);
        Assert.False(string.IsNullOrWhiteSpace(text));
    }
}

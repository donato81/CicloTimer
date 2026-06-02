using CicloTimer.Localization;

namespace CicloTimer.Localization.Tests;

public sealed class LocalizationKeysTests
{
    [Fact]
    public void TimerTextKeyExistsAndIsNotEmpty()
    {
        Assert.NotEmpty(Enum.GetValues<TimerTextKey>());
    }

    [Fact]
    public void CommandTextKeyExistsAndIsNotEmpty()
    {
        Assert.NotEmpty(Enum.GetValues<CommandTextKey>());
    }

    [Fact]
    public void ErrorTextKeyExistsAndIsNotEmpty()
    {
        Assert.NotEmpty(Enum.GetValues<ErrorTextKey>());
    }

    [Fact]
    public void AccessibilityTextKeyExistsAndIsNotEmpty()
    {
        Assert.NotEmpty(Enum.GetValues<AccessibilityTextKey>());
    }

    [Fact]
    public void UiTextKeyExistsAndIsNotEmpty()
    {
        Assert.NotEmpty(Enum.GetValues<UiTextKey>());
    }
}

using CicloTimer.Localization.Locales.It;

namespace CicloTimer.Localization.Tests;

public sealed class ItalianCommandTextsTests
{
    [Theory]
    [InlineData(CommandTextKey.Start, "Avvia")]
    [InlineData(CommandTextKey.Pause, "Pausa")]
    [InlineData(CommandTextKey.Resume, "Riprendi")]
    [InlineData(CommandTextKey.Reset, "Reset")]
    [InlineData(CommandTextKey.Configure, "Configura")]
    public void GetReturnsExpectedItalianCommandText(CommandTextKey key, string expected)
    {
        Assert.Equal(expected, ItalianCommandTexts.Get(key));
    }
}

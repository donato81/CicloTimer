using CicloTimer.Localization;

namespace CicloTimer.Localization.Tests;

public sealed class SupportedLanguageTests
{
    [Fact]
    public void SupportedLanguageContainsOnlyItalian()
    {
        var values = Enum.GetValues<SupportedLanguage>();

        var language = Assert.Single(values);
        Assert.Equal(SupportedLanguage.It, language);
    }

    [Fact]
    public void SupportedLanguageDoesNotContainFutureLanguages()
    {
        var names = Enum.GetNames<SupportedLanguage>();

        Assert.DoesNotContain("En", names);
        Assert.DoesNotContain("Fr", names);
        Assert.DoesNotContain("Es", names);
    }
}

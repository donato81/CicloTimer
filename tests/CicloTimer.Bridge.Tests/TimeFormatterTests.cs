using Xunit;

namespace CicloTimer.Bridge.Tests;

public sealed class TimeFormatterTests
{
    [Theory]
    [InlineData(0, "00:00")]
    [InlineData(5, "00:05")]
    [InlineData(59, "00:59")]
    [InlineData(60, "01:00")]
    [InlineData(299, "04:59")]
    [InlineData(300, "05:00")]
    [InlineData(3600, "60:00")]
    [InlineData(3661, "61:01")]
    [InlineData(-1, "00:00")]
    public void Format_ReturnsExpectedMmSsText(int seconds, string expected)
    {
        Assert.Equal(expected, TimeFormatter.Format(seconds));
    }
}

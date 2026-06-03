namespace CicloTimer.Bridge;

public static class TimeFormatter
{
    public static string Format(int seconds)
    {
        var normalizedSeconds = Math.Max(0, seconds);
        var minutes = normalizedSeconds / 60;
        var remainingSeconds = normalizedSeconds % 60;

        return $"{minutes:00}:{remainingSeconds:00}";
    }
}

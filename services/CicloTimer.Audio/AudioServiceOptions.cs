namespace CicloTimer.Audio;

public sealed record AudioServiceOptions(string FinalAlertAudioPath)
{
    public static AudioServiceOptions Default { get; } = new(
        Path.Combine(AppContext.BaseDirectory, "Assets", "final-alert.wav"));
}

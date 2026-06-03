namespace CicloTimer.Audio;

public sealed class WindowsAudioFocusManager : IAudioFocusManager
{
    private readonly IAudioFocusManager fallback;

    public WindowsAudioFocusManager()
        : this(new NullAudioFocusManager())
    {
    }

    internal WindowsAudioFocusManager(IAudioFocusManager fallback)
    {
        this.fallback = fallback;
    }

    public AudioActionResult TryApplyFocus()
    {
        return fallback.TryApplyFocus();
    }

    public AudioActionResult TryRestoreFocus()
    {
        return fallback.TryRestoreFocus();
    }
}

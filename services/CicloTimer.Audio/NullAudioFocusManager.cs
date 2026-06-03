namespace CicloTimer.Audio;

public sealed class NullAudioFocusManager : IAudioFocusManager
{
    public AudioActionResult TryApplyFocus()
    {
        return AudioActionResult.AudioFocusUnavailable;
    }

    public AudioActionResult TryRestoreFocus()
    {
        return AudioActionResult.Success;
    }
}

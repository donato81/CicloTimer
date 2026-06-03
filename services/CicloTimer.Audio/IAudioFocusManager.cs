namespace CicloTimer.Audio;

public interface IAudioFocusManager
{
    AudioActionResult TryApplyFocus();

    AudioActionResult TryRestoreFocus();
}

namespace CicloTimer.Audio;

public enum AudioActionResult
{
    NotAttempted,
    Success,
    AlreadyPlaying,
    AlreadyStopped,
    AudioFileMissing,
    PlaybackFailed,
    AudioFocusUnavailable,
    AudioFocusFailed,
    RestoreFailed
}

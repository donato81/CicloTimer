namespace CicloTimer.Audio;

public sealed class AudioService
{
    private readonly IAudioPlayer audioPlayer;
    private readonly IAudioFocusManager audioFocusManager;
    private readonly IAudioModificationTracker modificationTracker;
    private readonly AudioServiceOptions options;

    public AudioService()
        : this(
            new WindowsAudioPlayer(),
            new WindowsAudioFocusManager(),
            new AudioModificationTracker(),
            AudioServiceOptions.Default)
    {
    }

    public AudioService(
        IAudioPlayer audioPlayer,
        IAudioFocusManager audioFocusManager,
        IAudioModificationTracker modificationTracker,
        AudioServiceOptions options)
    {
        this.audioPlayer = audioPlayer;
        this.audioFocusManager = audioFocusManager;
        this.modificationTracker = modificationTracker;
        this.options = options;
    }

    public AudioPlaybackState State { get; private set; } = AudioPlaybackState.Idle;

    public AudioServiceResult StartFinalAlertSound()
    {
        if (State == AudioPlaybackState.PlayingFinalAlert)
        {
            return new AudioServiceResult(
                AudioActionResult.AlreadyPlaying,
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted);
        }

        if (!File.Exists(options.FinalAlertAudioPath))
        {
            State = AudioPlaybackState.Idle;
            return new AudioServiceResult(
                AudioActionResult.AudioFileMissing,
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted);
        }

        AudioActionResult playbackResult;
        try
        {
            playbackResult = audioPlayer.StartLoop(options.FinalAlertAudioPath);
        }
        catch (Exception)
        {
            playbackResult = AudioActionResult.PlaybackFailed;
        }

        if (playbackResult != AudioActionResult.Success)
        {
            State = playbackResult == AudioActionResult.AlreadyPlaying
                ? AudioPlaybackState.PlayingFinalAlert
                : AudioPlaybackState.Failed;

            return new AudioServiceResult(
                NormalizePlaybackFailure(playbackResult),
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted);
        }

        State = AudioPlaybackState.PlayingFinalAlert;

        AudioActionResult focusResult;
        try
        {
            focusResult = audioFocusManager.TryApplyFocus();
        }
        catch (Exception)
        {
            focusResult = AudioActionResult.AudioFocusFailed;
        }

        return new AudioServiceResult(
            AudioActionResult.Success,
            NormalizeFocusFailure(focusResult),
            AudioActionResult.NotAttempted);
    }

    public AudioServiceResult StopFinalAlertSound()
    {
        if (State == AudioPlaybackState.Idle)
        {
            return new AudioServiceResult(
                AudioActionResult.AlreadyStopped,
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted);
        }

        State = AudioPlaybackState.Stopping;

        AudioActionResult playbackResult;
        try
        {
            playbackResult = audioPlayer.Stop();
        }
        catch (Exception)
        {
            playbackResult = AudioActionResult.PlaybackFailed;
        }

        AudioActionResult restoreResult = AudioActionResult.NotAttempted;
        if (modificationTracker.HasTrackedModifications)
        {
            try
            {
                restoreResult = audioFocusManager.TryRestoreFocus();
            }
            catch (Exception)
            {
                restoreResult = AudioActionResult.RestoreFailed;
            }

            if (restoreResult == AudioActionResult.Success)
            {
                modificationTracker.Clear();
            }
        }

        State = playbackResult == AudioActionResult.PlaybackFailed
            ? AudioPlaybackState.Failed
            : AudioPlaybackState.Idle;

        return new AudioServiceResult(
            playbackResult,
            AudioActionResult.NotAttempted,
            NormalizeRestoreFailure(restoreResult));
    }

    private static AudioActionResult NormalizePlaybackFailure(AudioActionResult result)
    {
        return result switch
        {
            AudioActionResult.AudioFileMissing => AudioActionResult.AudioFileMissing,
            AudioActionResult.AlreadyPlaying => AudioActionResult.AlreadyPlaying,
            AudioActionResult.Success => AudioActionResult.Success,
            _ => AudioActionResult.PlaybackFailed
        };
    }

    private static AudioActionResult NormalizeFocusFailure(AudioActionResult result)
    {
        return result switch
        {
            AudioActionResult.Success => AudioActionResult.Success,
            AudioActionResult.AudioFocusUnavailable => AudioActionResult.AudioFocusUnavailable,
            _ => AudioActionResult.AudioFocusFailed
        };
    }

    private static AudioActionResult NormalizeRestoreFailure(AudioActionResult result)
    {
        return result switch
        {
            AudioActionResult.NotAttempted => AudioActionResult.NotAttempted,
            AudioActionResult.Success => AudioActionResult.Success,
            _ => AudioActionResult.RestoreFailed
        };
    }
}

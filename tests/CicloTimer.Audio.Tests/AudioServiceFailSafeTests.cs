using CicloTimer.Audio;

namespace CicloTimer.Audio.Tests;

public sealed class AudioServiceFailSafeTests
{
    [Fact]
    public void PlayerExceptionDuringStartIsCaptured()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new() { ThrowOnStart = true };
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.PlaybackFailed, result.PlaybackResult);
        Assert.Equal(AudioPlaybackState.Failed, service.State);
    }

    [Fact]
    public void PlayerExceptionDuringStopIsCaptured()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new() { ThrowOnStop = true };
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.PlaybackFailed, result.PlaybackResult);
        Assert.Equal(AudioPlaybackState.Failed, service.State);
    }

    [Fact]
    public void FocusManagerExceptionDuringApplyIsCaptured()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new() { ThrowOnApply = true };
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.PlaybackResult);
        Assert.Equal(AudioActionResult.AudioFocusFailed, result.FocusResult);
        Assert.Equal(AudioPlaybackState.PlayingFinalAlert, service.State);
    }

    [Fact]
    public void FocusManagerExceptionDuringRestoreIsCaptured()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new() { ThrowOnRestore = true };
        AudioModificationTracker tracker = new();
        tracker.Track(new AudioModificationSnapshot("test", "test"));
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager, tracker: tracker);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.RestoreFailed, result.RestoreResult);
    }

    [Fact]
    public void MissingFileDoesNotThrowUnhandledException()
    {
        string audioPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.wav");
        AudioService service = AudioServiceTestFactory.Create(audioPath);

        Exception? exception = Record.Exception(() => service.StartFinalAlertSound());

        Assert.Null(exception);
    }

    [Fact]
    public void FocusFailureDoesNotBlockSuccessfulPlayback()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new()
        {
            ApplyResult = AudioActionResult.AudioFocusFailed
        };
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.PlaybackResult);
        Assert.Equal(AudioActionResult.AudioFocusFailed, result.FocusResult);
        Assert.Equal(AudioPlaybackState.PlayingFinalAlert, service.State);
    }

    [Fact]
    public void RestoreFailureProducesControlledResult()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new()
        {
            RestoreResult = AudioActionResult.RestoreFailed
        };
        AudioModificationTracker tracker = new();
        tracker.Track(new AudioModificationSnapshot("test", "test"));
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager, tracker: tracker);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.RestoreFailed, result.RestoreResult);
        Assert.Equal(AudioPlaybackState.Idle, service.State);
    }
}

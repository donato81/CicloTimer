using CicloTimer.Audio;

namespace CicloTimer.Audio.Tests;

public sealed class AudioServiceStateTests
{
    [Fact]
    public void NewServiceStartsIdle()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        AudioService service = AudioServiceTestFactory.Create(audioPath);

        Assert.Equal(AudioPlaybackState.Idle, service.State);
    }

    [Fact]
    public void ValidStartReturnsSuccessAndSetsPlayingFinalAlert()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        AudioService service = AudioServiceTestFactory.Create(audioPath);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.PlaybackResult);
        Assert.Equal(AudioPlaybackState.PlayingFinalAlert, service.State);
    }

    [Fact]
    public void ValidStopReturnsIdle()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        AudioService service = AudioServiceTestFactory.Create(audioPath);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.PlaybackResult);
        Assert.Equal(AudioPlaybackState.Idle, service.State);
    }

    [Fact]
    public void MissingFileReturnsControlledResultAndSafeState()
    {
        string audioPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.wav");
        AudioService service = AudioServiceTestFactory.Create(audioPath);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.AudioFileMissing, result.PlaybackResult);
        Assert.Equal(AudioPlaybackState.Idle, service.State);
    }

    [Fact]
    public void PlaybackFailureReturnsControlledResultAndSafeState()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new() { StartLoopResult = AudioActionResult.PlaybackFailed };
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.PlaybackFailed, result.PlaybackResult);
        Assert.Equal(AudioPlaybackState.Failed, service.State);
    }

    [Fact]
    public void FocusUnavailableDoesNotBlockSuccessfulPlayback()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new()
        {
            ApplyResult = AudioActionResult.AudioFocusUnavailable
        };
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.PlaybackResult);
        Assert.Equal(AudioActionResult.AudioFocusUnavailable, result.FocusResult);
        Assert.Equal(AudioActionResult.NotAttempted, result.RestoreResult);
        Assert.Equal(AudioPlaybackState.PlayingFinalAlert, service.State);
    }
}

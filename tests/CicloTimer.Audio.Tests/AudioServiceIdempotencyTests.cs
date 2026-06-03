using CicloTimer.Audio;

namespace CicloTimer.Audio.Tests;

public sealed class AudioServiceIdempotencyTests
{
    [Fact]
    public void StartFromIdleStartsOnePlayback()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new();
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        service.StartFinalAlertSound();

        Assert.Equal(1, player.StartLoopCalls);
    }

    [Fact]
    public void RepeatedStartWhilePlayingDoesNotStartDuplicate()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new();
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.AlreadyPlaying, result.PlaybackResult);
        Assert.Equal(1, player.StartLoopCalls);
        Assert.Equal(AudioPlaybackState.PlayingFinalAlert, service.State);
    }

    [Fact]
    public void StopWhilePlayingStopsPlayer()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new();
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.PlaybackResult);
        Assert.Equal(1, player.StopCalls);
        Assert.Equal(AudioPlaybackState.Idle, service.State);
    }

    [Fact]
    public void RepeatedStopReturnsAlreadyStopped()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new();
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        service.StartFinalAlertSound();
        service.StopFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.AlreadyStopped, result.PlaybackResult);
        Assert.Equal(1, player.StopCalls);
    }

    [Fact]
    public void StopFromIdleDoesNotCallPlayerOrFocus()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new();
        FakeAudioFocusManager focusManager = new();
        AudioService service = AudioServiceTestFactory.Create(audioPath, player, focusManager);

        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.AlreadyStopped, result.PlaybackResult);
        Assert.Equal(0, player.StopCalls);
        Assert.Equal(0, focusManager.RestoreCalls);
    }

    [Fact]
    public void RepeatedStartDoesNotCreateLogicalOverlap()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioPlayer player = new();
        AudioService service = AudioServiceTestFactory.Create(audioPath, player);

        service.StartFinalAlertSound();
        service.StartFinalAlertSound();
        service.StartFinalAlertSound();

        Assert.Equal(1, player.StartLoopCalls);
        Assert.Equal(AudioPlaybackState.PlayingFinalAlert, service.State);
    }
}

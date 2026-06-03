using CicloTimer.Audio;

namespace CicloTimer.Audio.Tests;

public sealed class AudioServiceFocusTests
{
    [Fact]
    public void NullAudioFocusManagerApplyReturnsUnavailable()
    {
        NullAudioFocusManager focusManager = new();

        AudioActionResult result = focusManager.TryApplyFocus();

        Assert.Equal(AudioActionResult.AudioFocusUnavailable, result);
    }

    [Fact]
    public void NullAudioFocusManagerRestoreDoesNotFail()
    {
        NullAudioFocusManager focusManager = new();

        AudioActionResult result = focusManager.TryRestoreFocus();

        Assert.Equal(AudioActionResult.Success, result);
    }

    [Fact]
    public void FocusUnavailableDoesNotPreventStart()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        AudioService service = AudioServiceTestFactory.Create(
            audioPath,
            focusManager: new FakeAudioFocusManager { ApplyResult = AudioActionResult.AudioFocusUnavailable });

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.PlaybackResult);
        Assert.Equal(AudioActionResult.AudioFocusUnavailable, result.FocusResult);
    }

    [Fact]
    public void TrackerTracksOnlyWhenExplicitlyAsked()
    {
        AudioModificationTracker tracker = new();

        Assert.False(tracker.HasTrackedModifications);

        tracker.Track(new AudioModificationSnapshot("safe-scope", "technical-test"));

        Assert.True(tracker.HasTrackedModifications);
        Assert.Single(tracker.GetTrackedModifications());
    }

    [Fact]
    public void StopWithoutTrackedModificationsDoesNotRestore()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new();
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.NotAttempted, result.RestoreResult);
        Assert.Equal(0, focusManager.RestoreCalls);
    }

    [Fact]
    public void StopWithTrackedModificationsAttemptsRestore()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new();
        AudioModificationTracker tracker = new();
        tracker.Track(new AudioModificationSnapshot("safe-scope", "technical-test"));
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager, tracker: tracker);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.Success, result.RestoreResult);
        Assert.Equal(1, focusManager.RestoreCalls);
    }

    [Fact]
    public void SuccessfulRestoreClearsTrackedModifications()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        AudioModificationTracker tracker = new();
        tracker.Track(new AudioModificationSnapshot("safe-scope", "technical-test"));
        AudioService service = AudioServiceTestFactory.Create(audioPath, tracker: tracker);

        service.StartFinalAlertSound();
        service.StopFinalAlertSound();

        Assert.False(tracker.HasTrackedModifications);
    }

    [Fact]
    public void FailedRestoreKeepsControlledFailure()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        FakeAudioFocusManager focusManager = new()
        {
            RestoreResult = AudioActionResult.RestoreFailed
        };
        AudioModificationTracker tracker = new();
        tracker.Track(new AudioModificationSnapshot("safe-scope", "technical-test"));
        AudioService service = AudioServiceTestFactory.Create(audioPath, focusManager: focusManager, tracker: tracker);

        service.StartFinalAlertSound();
        AudioServiceResult result = service.StopFinalAlertSound();

        Assert.Equal(AudioActionResult.RestoreFailed, result.RestoreResult);
    }

    [Fact]
    public void TrackerCanRemainEmptyWithNullFocusManager()
    {
        string audioPath = AudioServiceTestFactory.CreateExistingAudioFile();
        AudioModificationTracker tracker = new();
        AudioService service = new(
            new FakeAudioPlayer(),
            new NullAudioFocusManager(),
            tracker,
            new AudioServiceOptions(audioPath));

        service.StartFinalAlertSound();

        Assert.False(tracker.HasTrackedModifications);
    }
}

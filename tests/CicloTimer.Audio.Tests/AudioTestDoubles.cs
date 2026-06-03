using CicloTimer.Audio;

namespace CicloTimer.Audio.Tests;

internal sealed class FakeAudioPlayer : IAudioPlayer
{
    public int StartLoopCalls { get; private set; }

    public int StopCalls { get; private set; }

    public bool IsPlaying { get; private set; }

    public AudioActionResult StartLoopResult { get; set; } = AudioActionResult.Success;

    public AudioActionResult StopResult { get; set; } = AudioActionResult.Success;

    public bool ThrowOnStart { get; set; }

    public bool ThrowOnStop { get; set; }

    public AudioActionResult StartLoop(string audioFilePath)
    {
        StartLoopCalls++;

        if (ThrowOnStart)
        {
            throw new InvalidOperationException("Technical test exception");
        }

        if (StartLoopResult == AudioActionResult.Success)
        {
            IsPlaying = true;
        }

        return StartLoopResult;
    }

    public AudioActionResult Stop()
    {
        StopCalls++;

        if (ThrowOnStop)
        {
            throw new InvalidOperationException("Technical test exception");
        }

        if (StopResult == AudioActionResult.Success)
        {
            IsPlaying = false;
        }

        return StopResult;
    }
}

internal sealed class FakeAudioFocusManager : IAudioFocusManager
{
    public int ApplyCalls { get; private set; }

    public int RestoreCalls { get; private set; }

    public AudioActionResult ApplyResult { get; set; } = AudioActionResult.AudioFocusUnavailable;

    public AudioActionResult RestoreResult { get; set; } = AudioActionResult.Success;

    public bool ThrowOnApply { get; set; }

    public bool ThrowOnRestore { get; set; }

    public AudioActionResult TryApplyFocus()
    {
        ApplyCalls++;

        if (ThrowOnApply)
        {
            throw new InvalidOperationException("Technical test exception");
        }

        return ApplyResult;
    }

    public AudioActionResult TryRestoreFocus()
    {
        RestoreCalls++;

        if (ThrowOnRestore)
        {
            throw new InvalidOperationException("Technical test exception");
        }

        return RestoreResult;
    }
}

internal static class AudioServiceTestFactory
{
    public static AudioService Create(
        string audioPath,
        FakeAudioPlayer? player = null,
        FakeAudioFocusManager? focusManager = null,
        IAudioModificationTracker? tracker = null)
    {
        return new AudioService(
            player ?? new FakeAudioPlayer(),
            focusManager ?? new FakeAudioFocusManager(),
            tracker ?? new AudioModificationTracker(),
            new AudioServiceOptions(audioPath));
    }

    public static string CreateExistingAudioFile()
    {
        string path = Path.Combine(Path.GetTempPath(), $"ciclotimer-audio-test-{Guid.NewGuid():N}.wav");
        File.WriteAllBytes(path, [1, 2, 3, 4]);
        return path;
    }
}

namespace CicloTimer.Audio;

public interface IAudioPlayer
{
    AudioActionResult StartLoop(string audioFilePath);

    AudioActionResult Stop();

    bool IsPlaying { get; }
}

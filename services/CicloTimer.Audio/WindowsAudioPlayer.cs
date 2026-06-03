using System.Media;

namespace CicloTimer.Audio;

public sealed class WindowsAudioPlayer : IAudioPlayer, IDisposable
{
    private SoundPlayer? soundPlayer;

    public bool IsPlaying { get; private set; }

    public AudioActionResult StartLoop(string audioFilePath)
    {
        if (IsPlaying)
        {
            return AudioActionResult.AlreadyPlaying;
        }

        if (!File.Exists(audioFilePath))
        {
            return AudioActionResult.AudioFileMissing;
        }

        try
        {
            soundPlayer?.Dispose();
            soundPlayer = new SoundPlayer(audioFilePath);
            soundPlayer.Load();
            soundPlayer.PlayLooping();
            IsPlaying = true;
            return AudioActionResult.Success;
        }
        catch (Exception)
        {
            IsPlaying = false;
            soundPlayer?.Dispose();
            soundPlayer = null;
            return AudioActionResult.PlaybackFailed;
        }
    }

    public AudioActionResult Stop()
    {
        if (!IsPlaying)
        {
            return AudioActionResult.AlreadyStopped;
        }

        try
        {
            soundPlayer?.Stop();
            return AudioActionResult.Success;
        }
        catch (Exception)
        {
            return AudioActionResult.PlaybackFailed;
        }
        finally
        {
            IsPlaying = false;
            soundPlayer?.Dispose();
            soundPlayer = null;
        }
    }

    public void Dispose()
    {
        soundPlayer?.Dispose();
    }
}

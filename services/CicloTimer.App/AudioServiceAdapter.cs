using CicloTimer.Audio;

namespace CicloTimer.App;

public sealed class AudioServiceAdapter : IAudioServicePort
{
    private readonly AudioService audioService;

    public AudioServiceAdapter(AudioService audioService)
    {
        this.audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
    }

    public AudioServiceResult StartFinalAlertSound()
    {
        return audioService.StartFinalAlertSound();
    }

    public AudioServiceResult StopFinalAlertSound()
    {
        return audioService.StopFinalAlertSound();
    }
}

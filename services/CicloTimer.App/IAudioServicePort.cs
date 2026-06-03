using CicloTimer.Audio;

namespace CicloTimer.App;

public interface IAudioServicePort
{
    AudioServiceResult StartFinalAlertSound();

    AudioServiceResult StopFinalAlertSound();
}

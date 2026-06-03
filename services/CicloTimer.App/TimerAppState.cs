using CicloTimer.Audio;
using CicloTimer.Bridge;

namespace CicloTimer.App;

public sealed record TimerAppState(
    TimerDisplayModel CurrentModel,
    AudioServiceResult? LastAudioResult,
    AppCommandResult? LastCommandResult);

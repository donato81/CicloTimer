using CicloTimer.Audio;
using CicloTimer.Bridge;

namespace CicloTimer.App;

public sealed record AppCommandResult(
    TimerDisplayModel CurrentModel,
    AudioServiceResult? LastAudioResult,
    bool Success,
    bool HasAudioWarning,
    bool HasTechnicalError,
    int UnhandledActionCount);

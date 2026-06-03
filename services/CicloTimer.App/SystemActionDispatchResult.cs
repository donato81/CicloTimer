using CicloTimer.Audio;

namespace CicloTimer.App;

public sealed record SystemActionDispatchResult(
    AudioServiceResult? LastAudioResult,
    int ExecutedActions,
    int IgnoredActions,
    bool HasWarning,
    bool HasTechnicalError);

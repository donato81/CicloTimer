using CicloTimer.Core.Timer;
using CicloTimer.Localization;

namespace CicloTimer.Bridge;

public sealed class PrimaryActionResolver
{
    private readonly LocalizationService _localization;

    public PrimaryActionResolver(LocalizationService? localization = null)
    {
        _localization = localization ?? new LocalizationService();
    }

    public string Resolve(TimerState state, bool canStart, bool canPause, bool canResume, bool canReset)
    {
        _ = canReset;

        return state switch
        {
            TimerState.Stopped when canStart => _localization.GetCommandText(CommandTextKey.Start),
            TimerState.Running when canPause => _localization.GetCommandText(CommandTextKey.Pause),
            TimerState.FinalAlert when canPause => _localization.GetCommandText(CommandTextKey.Pause),
            TimerState.Paused when canResume => _localization.GetCommandText(CommandTextKey.Resume),
            _ => string.Empty
        };
    }
}

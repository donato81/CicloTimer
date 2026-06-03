using CicloTimer.Core.Timer;
using CicloTimer.Localization;

namespace CicloTimer.Bridge;

public sealed class TimerStateTextMapper
{
    private readonly LocalizationService _localization;

    public TimerStateTextMapper(LocalizationService? localization = null)
    {
        _localization = localization ?? new LocalizationService();
    }

    public string Map(TimerState state)
    {
        return _localization.GetTimerText(ToTextKey(state));
    }

    private static TimerTextKey ToTextKey(TimerState state)
    {
        return state switch
        {
            TimerState.Stopped => TimerTextKey.StateStopped,
            TimerState.Running => TimerTextKey.StateRunning,
            TimerState.FinalAlert => TimerTextKey.StateFinalAlert,
            TimerState.Paused => TimerTextKey.StatePaused,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Timer state is not supported.")
        };
    }
}

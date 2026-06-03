using CicloTimer.Core.Timer;
using CicloTimer.Localization;

namespace CicloTimer.Bridge;

public sealed class TimerErrorTextMapper
{
    private readonly LocalizationService _localization;

    public TimerErrorTextMapper(LocalizationService? localization = null)
    {
        _localization = localization ?? new LocalizationService();
    }

    public string Map(IReadOnlyList<TimerError> errors)
    {
        if (errors.Count == 0)
        {
            return string.Empty;
        }

        return _localization.GetErrorText(ToTextKey(errors[0]));
    }

    private static ErrorTextKey ToTextKey(TimerError error)
    {
        return error switch
        {
            TimerError.InvalidSessionDuration => ErrorTextKey.InvalidSessionDuration,
            TimerError.InvalidFinalAlertDuration => ErrorTextKey.InvalidFinalAlertDuration,
            TimerError.FinalAlertNotLessThanSessionDuration => ErrorTextKey.FinalAlertNotLessThanSessionDuration,
            TimerError.TimerNotConfigured => ErrorTextKey.TimerNotConfigured,
            TimerError.CannotStart => ErrorTextKey.CannotStart,
            TimerError.CannotPause => ErrorTextKey.CannotPause,
            TimerError.CannotResume => ErrorTextKey.CannotResume,
            TimerError.CannotReset => ErrorTextKey.CannotReset,
            TimerError.InvalidTickDuration => ErrorTextKey.InvalidTickDuration,
            _ => throw new ArgumentOutOfRangeException(nameof(error), error, "Timer error is not supported.")
        };
    }
}

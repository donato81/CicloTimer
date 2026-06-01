namespace CicloTimer.Core.Timer;

public enum TimerError
{
    InvalidSessionDuration,
    InvalidFinalAlertDuration,
    FinalAlertNotLessThanSessionDuration,
    TimerNotConfigured,
    CannotStart,
    CannotPause,
    CannotResume,
    CannotReset,
    InvalidTickDuration
}

namespace CicloTimer.Localization;

public enum TimerTextKey
{
    StateStopped,
    StateRunning,
    StateFinalAlert,
    StatePaused,
    EventTimerConfigured,
    EventTimerStarted,
    EventTimerPaused,
    EventTimerResumed,
    EventTimerReset,
    EventFinalAlertStarted,
    EventSessionCompleted,
    EventSessionCounterIncremented,
    EventNextSessionStarted,
    EventValidationFailed
}

public enum CommandTextKey
{
    Start,
    Pause,
    Resume,
    Reset,
    Configure
}

public enum ErrorTextKey
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

public enum AccessibilityTextKey
{
    StatusTemplate,
    SessionCompletedTemplate,
    ErrorTemplate,
    StartTimer,
    PauseTimer,
    ResumeTimer,
    ResetTimer,
    SessionDurationMinutes,
    SessionDurationSeconds,
    FinalAlertDurationSeconds
}

public enum UiTextKey
{
    AppTitle,
    SessionDuration,
    FinalAlertDuration,
    Minutes,
    Seconds,
    RemainingTime,
    TimerState,
    CompletedSessions,
    Message
}

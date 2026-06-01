namespace CicloTimer.Core.Timer;

public enum TimerEvent
{
    TimerConfigured,
    TimerStarted,
    TimerPaused,
    TimerResumed,
    TimerReset,
    FinalAlertStarted,
    SessionCompleted,
    SessionCounterIncremented,
    NextSessionStarted,
    ValidationFailed
}

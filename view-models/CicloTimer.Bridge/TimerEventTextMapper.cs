using CicloTimer.Core.Timer;
using CicloTimer.Localization;

namespace CicloTimer.Bridge;

public sealed class TimerEventTextMapper
{
    private readonly LocalizationService _localization;

    public TimerEventTextMapper(LocalizationService? localization = null)
    {
        _localization = localization ?? new LocalizationService();
    }

    public string Map(IReadOnlyList<TimerEvent> events, int completedSessions)
    {
        if (events.Count == 0)
        {
            return string.Empty;
        }

        if (ContainsSessionCompletionSequence(events))
        {
            return _localization.GetAccessibilityText(
                AccessibilityTextKey.SessionCompletedTemplate,
                null,
                completedSessions);
        }

        return _localization.GetTimerText(ToTextKey(events[0]));
    }

    private static bool ContainsSessionCompletionSequence(IReadOnlyList<TimerEvent> events)
    {
        return events.Contains(TimerEvent.SessionCompleted)
            && events.Contains(TimerEvent.SessionCounterIncremented)
            && events.Contains(TimerEvent.NextSessionStarted);
    }

    private static TimerTextKey ToTextKey(TimerEvent timerEvent)
    {
        return timerEvent switch
        {
            TimerEvent.TimerConfigured => TimerTextKey.EventTimerConfigured,
            TimerEvent.TimerStarted => TimerTextKey.EventTimerStarted,
            TimerEvent.TimerPaused => TimerTextKey.EventTimerPaused,
            TimerEvent.TimerResumed => TimerTextKey.EventTimerResumed,
            TimerEvent.TimerReset => TimerTextKey.EventTimerReset,
            TimerEvent.FinalAlertStarted => TimerTextKey.EventFinalAlertStarted,
            TimerEvent.SessionCompleted => TimerTextKey.EventSessionCompleted,
            TimerEvent.SessionCounterIncremented => TimerTextKey.EventSessionCounterIncremented,
            TimerEvent.NextSessionStarted => TimerTextKey.EventNextSessionStarted,
            TimerEvent.ValidationFailed => TimerTextKey.EventValidationFailed,
            _ => throw new ArgumentOutOfRangeException(nameof(timerEvent), timerEvent, "Timer event is not supported.")
        };
    }
}

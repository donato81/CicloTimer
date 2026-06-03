using CicloTimer.Core.Timer;

namespace CicloTimer.Bridge;

public sealed class SystemActionResolver
{
    public IReadOnlyList<SystemActionRequest> Resolve(
        IReadOnlyList<TimerEvent> events,
        bool wasFinalAlertActive,
        bool isFinalAlertActive)
    {
        var actions = new List<SystemActionRequest>();

        if (events.Contains(TimerEvent.FinalAlertStarted))
        {
            actions.Add(SystemActionRequest.StartFinalAlertSound);
        }

        if (ShouldStopFinalAlert(events, wasFinalAlertActive, isFinalAlertActive))
        {
            actions.Add(SystemActionRequest.StopFinalAlertSound);
        }

        return actions;
    }

    private static bool ShouldStopFinalAlert(
        IReadOnlyList<TimerEvent> events,
        bool wasFinalAlertActive,
        bool isFinalAlertActive)
    {
        var hasSessionCompletionEvent = events.Contains(TimerEvent.SessionCompleted)
            || events.Contains(TimerEvent.SessionCounterIncremented)
            || events.Contains(TimerEvent.NextSessionStarted);

        if (hasSessionCompletionEvent && (wasFinalAlertActive || isFinalAlertActive))
        {
            return true;
        }

        if (!wasFinalAlertActive)
        {
            return false;
        }

        return events.Contains(TimerEvent.TimerPaused)
            || events.Contains(TimerEvent.TimerReset);
    }
}

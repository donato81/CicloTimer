namespace CicloTimer.Core.Timer;

public sealed class TimerCommandResult
{
    public bool Success { get; init; }
    public TimerState State { get; init; }
    public int RemainingSeconds { get; init; }
    public int CompletedSessions { get; init; }
    public bool IsConfigured { get; init; }
    public IReadOnlyList<TimerError> Errors { get; init; } = [];
    public IReadOnlyList<TimerEvent> Events { get; init; } = [];
}

namespace CicloTimer.Bridge;

public sealed record TimerBridgeUpdate
{
    public TimerBridgeUpdate(TimerDisplayModel DisplayModel, IReadOnlyList<SystemActionRequest>? SystemActions)
    {
        this.DisplayModel = DisplayModel ?? throw new ArgumentNullException(nameof(DisplayModel));
        this.SystemActions = SystemActions ?? Array.Empty<SystemActionRequest>();
    }

    public TimerDisplayModel DisplayModel { get; }
    public IReadOnlyList<SystemActionRequest> SystemActions { get; }
}

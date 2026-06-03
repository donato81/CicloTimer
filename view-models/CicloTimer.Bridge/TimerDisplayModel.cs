namespace CicloTimer.Bridge;

public sealed record TimerDisplayModel
{
    public TimerDisplayModel(
        string RemainingTimeText,
        string TimerStateText,
        string CompletedSessionsText,
        string PrimaryActionText,
        bool CanStart,
        bool CanPause,
        bool CanResume,
        bool CanReset,
        bool IsConfigured,
        bool IsFinalAlertActive,
        string ErrorMessageText,
        string EventMessageText,
        string AccessibleStatusText,
        string AccessibleEventText)
    {
        this.RemainingTimeText = RemainingTimeText ?? string.Empty;
        this.TimerStateText = TimerStateText ?? string.Empty;
        this.CompletedSessionsText = CompletedSessionsText ?? string.Empty;
        this.PrimaryActionText = PrimaryActionText ?? string.Empty;
        this.CanStart = CanStart;
        this.CanPause = CanPause;
        this.CanResume = CanResume;
        this.CanReset = CanReset;
        this.IsConfigured = IsConfigured;
        this.IsFinalAlertActive = IsFinalAlertActive;
        this.ErrorMessageText = ErrorMessageText ?? string.Empty;
        this.EventMessageText = EventMessageText ?? string.Empty;
        this.AccessibleStatusText = AccessibleStatusText ?? string.Empty;
        this.AccessibleEventText = AccessibleEventText ?? string.Empty;
    }

    public string RemainingTimeText { get; }
    public string TimerStateText { get; }
    public string CompletedSessionsText { get; }
    public string PrimaryActionText { get; }
    public bool CanStart { get; }
    public bool CanPause { get; }
    public bool CanResume { get; }
    public bool CanReset { get; }
    public bool IsConfigured { get; }
    public bool IsFinalAlertActive { get; }
    public string ErrorMessageText { get; }
    public string EventMessageText { get; }
    public string AccessibleStatusText { get; }
    public string AccessibleEventText { get; }
}

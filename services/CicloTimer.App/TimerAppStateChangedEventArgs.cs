namespace CicloTimer.App;

public sealed class TimerAppStateChangedEventArgs : EventArgs
{
    public TimerAppStateChangedEventArgs(TimerAppState state, AppCommandResult result)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public TimerAppState State { get; }

    public AppCommandResult Result { get; }
}

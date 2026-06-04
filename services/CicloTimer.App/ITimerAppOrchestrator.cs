using CicloTimer.Bridge;

namespace CicloTimer.App;

public interface ITimerAppOrchestrator : IDisposable
{
    event EventHandler<TimerAppStateChangedEventArgs>? StateChanged;

    TimerAppState CurrentState { get; }

    AppCommandResult Configure(TimerInput input);

    AppCommandResult Start();

    AppCommandResult Pause();

    AppCommandResult Resume();

    AppCommandResult Reset();

    AppCommandResult Tick(int elapsedSeconds);
}

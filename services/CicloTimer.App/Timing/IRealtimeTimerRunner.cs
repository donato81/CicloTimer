namespace CicloTimer.App.Timing;

public interface IRealtimeTimerRunner : IDisposable
{
    bool IsRunning { get; }

    void Start();

    void Stop();
}

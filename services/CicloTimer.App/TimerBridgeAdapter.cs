using CicloTimer.Bridge;

namespace CicloTimer.App;

public sealed class TimerBridgeAdapter : ITimerBridgePort
{
    private readonly TimerBridge bridge;

    public TimerBridgeAdapter(TimerBridge bridge)
    {
        this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
    }

    public TimerBridgeUpdate GetCurrentUpdate()
    {
        return bridge.GetCurrentUpdate();
    }

    public TimerBridgeUpdate Configure(TimerInput input)
    {
        return bridge.Configure(input);
    }

    public TimerBridgeUpdate Start()
    {
        return bridge.Start();
    }

    public TimerBridgeUpdate Pause()
    {
        return bridge.Pause();
    }

    public TimerBridgeUpdate Resume()
    {
        return bridge.Resume();
    }

    public TimerBridgeUpdate Reset()
    {
        return bridge.Reset();
    }

    public TimerBridgeUpdate Tick(int elapsedSeconds)
    {
        return bridge.Tick(elapsedSeconds);
    }
}

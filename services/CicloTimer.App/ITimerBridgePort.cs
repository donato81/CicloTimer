using CicloTimer.Bridge;

namespace CicloTimer.App;

public interface ITimerBridgePort
{
    TimerBridgeUpdate GetCurrentUpdate();

    TimerBridgeUpdate Configure(TimerInput input);

    TimerBridgeUpdate Start();

    TimerBridgeUpdate Pause();

    TimerBridgeUpdate Resume();

    TimerBridgeUpdate Reset();

    TimerBridgeUpdate Tick(int elapsedSeconds);
}

namespace CicloTimer.Bridge;

public sealed record TimerInput(
    int SessionMinutes,
    int SessionSeconds,
    int FinalAlertMinutes,
    int FinalAlertSeconds);

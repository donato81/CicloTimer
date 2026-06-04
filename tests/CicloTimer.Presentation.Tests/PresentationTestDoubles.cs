using CicloTimer.App;
using CicloTimer.App.Timing;
using CicloTimer.Bridge;
using CicloTimer.ViewModels;

namespace CicloTimer.Presentation.Tests;

internal sealed class ImmediateUiDispatcher : IUiDispatcher
{
    public void Post(Action action)
    {
        action();
    }
}

internal sealed class FakeRealtimeTimerRunner : IRealtimeTimerRunner
{
    public bool IsRunning { get; private set; }
    public int StartCalls { get; private set; }
    public int StopCalls { get; private set; }
    public int DisposeCalls { get; private set; }

    public void Start()
    {
        StartCalls++;
        IsRunning = true;
    }

    public void Stop()
    {
        StopCalls++;
        IsRunning = false;
    }

    public void Dispose()
    {
        DisposeCalls++;
        IsRunning = false;
    }
}

internal sealed class FakeTimerAppOrchestrator : ITimerAppOrchestrator
{
    private EventHandler<TimerAppStateChangedEventArgs>? stateChanged;

    public event EventHandler<TimerAppStateChangedEventArgs>? StateChanged
    {
        add
        {
            StateChangedAddCount++;
            stateChanged += value;
        }
        remove
        {
            StateChangedRemoveCount++;
            stateChanged -= value;
        }
    }

    public TimerAppState CurrentState { get; private set; } = new(
        TestDisplayModels.Stopped(),
        LastAudioResult: null,
        LastCommandResult: null);

    public int ConfigureCalls { get; private set; }
    public int StartCalls { get; private set; }
    public int PauseCalls { get; private set; }
    public int ResumeCalls { get; private set; }
    public int ResetCalls { get; private set; }
    public int TickCalls { get; private set; }
    public int DisposeCalls { get; private set; }
    public int StateChangedAddCount { get; private set; }
    public int StateChangedRemoveCount { get; private set; }
    public TimerInput? LastInput { get; private set; }

    public AppCommandResult Configure(TimerInput input)
    {
        ConfigureCalls++;
        LastInput = input;
        return Update(TestDisplayModels.Stopped(input.SessionMinutes, input.SessionSeconds));
    }

    public AppCommandResult Start()
    {
        StartCalls++;
        return Update(TestDisplayModels.Running());
    }

    public AppCommandResult Pause()
    {
        PauseCalls++;
        return Update(TestDisplayModels.Paused());
    }

    public AppCommandResult Resume()
    {
        ResumeCalls++;
        return Update(TestDisplayModels.Running());
    }

    public AppCommandResult Reset()
    {
        ResetCalls++;
        return Update(TestDisplayModels.Stopped());
    }

    public AppCommandResult Tick(int elapsedSeconds)
    {
        TickCalls++;
        return Update(TestDisplayModels.Running(remainingTimeText: $"tick-{elapsedSeconds}"));
    }

    public void Dispose()
    {
        DisposeCalls++;
    }

    public void RaiseStateChanged(TimerDisplayModel model)
    {
        var result = BuildResult(model);
        CurrentState = new TimerAppState(model, LastAudioResult: null, result);
        stateChanged?.Invoke(this, new TimerAppStateChangedEventArgs(CurrentState, result));
    }

    private AppCommandResult Update(TimerDisplayModel model)
    {
        var result = BuildResult(model);
        CurrentState = new TimerAppState(model, LastAudioResult: null, result);
        stateChanged?.Invoke(this, new TimerAppStateChangedEventArgs(CurrentState, result));
        return result;
    }

    private static AppCommandResult BuildResult(TimerDisplayModel model)
    {
        return new AppCommandResult(
            model,
            LastAudioResult: null,
            Success: true,
            HasAudioWarning: false,
            HasTechnicalError: false,
            UnhandledActionCount: 0);
    }
}

internal static class TestDisplayModels
{
    public static TimerDisplayModel Stopped(int minutes = 25, int seconds = 0)
    {
        return Create(
            remainingTimeText: $"{minutes:00}:{seconds:00}",
            timerStateText: "Timer fermo",
            primaryActionText: "Avvia",
            canStart: true,
            canPause: false,
            canResume: false,
            canReset: true,
            isConfigured: true);
    }

    public static TimerDisplayModel Running(string remainingTimeText = "24:59")
    {
        return Create(
            remainingTimeText,
            timerStateText: "Sessione in corso",
            primaryActionText: "Pausa",
            canStart: false,
            canPause: true,
            canResume: false,
            canReset: true,
            isConfigured: true);
    }

    public static TimerDisplayModel Paused()
    {
        return Create(
            remainingTimeText: "24:59",
            timerStateText: "Timer in pausa",
            primaryActionText: "Riprendi",
            canStart: false,
            canPause: false,
            canResume: true,
            canReset: true,
            isConfigured: true);
    }

    private static TimerDisplayModel Create(
        string remainingTimeText,
        string timerStateText,
        string primaryActionText,
        bool canStart,
        bool canPause,
        bool canResume,
        bool canReset,
        bool isConfigured)
    {
        return new TimerDisplayModel(
            remainingTimeText,
            timerStateText,
            "Sessioni completate: 0",
            primaryActionText,
            canStart,
            canPause,
            canResume,
            canReset,
            isConfigured,
            IsFinalAlertActive: false,
            ErrorMessageText: string.Empty,
            EventMessageText: string.Empty,
            AccessibleStatusText: timerStateText,
            AccessibleEventText: string.Empty);
    }
}

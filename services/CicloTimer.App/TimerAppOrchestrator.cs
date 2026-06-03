using CicloTimer.Audio;
using CicloTimer.Bridge;

namespace CicloTimer.App;

public sealed class TimerAppOrchestrator : ITimerAppOrchestrator
{
    private readonly object _sync = new();
    private readonly ITimerBridgePort bridge;
    private readonly SystemActionDispatcher systemActionDispatcher;
    private TimerAppState currentState;
    private bool disposed;

    public TimerAppOrchestrator(
        ITimerBridgePort bridge,
        IAudioServicePort audioService)
        : this(bridge, new SystemActionDispatcher(audioService))
    {
    }

    public TimerAppOrchestrator(
        ITimerBridgePort bridge,
        SystemActionDispatcher systemActionDispatcher)
    {
        this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
        this.systemActionDispatcher = systemActionDispatcher
            ?? throw new ArgumentNullException(nameof(systemActionDispatcher));

        var initialUpdate = this.bridge.GetCurrentUpdate();
        currentState = new TimerAppState(
            initialUpdate.DisplayModel,
            LastAudioResult: null,
            LastCommandResult: null);
    }

    public TimerAppState CurrentState
    {
        get
        {
            lock (_sync)
            {
                return currentState;
            }
        }
    }

    public AppCommandResult Configure(TimerInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return ExecuteCommand(() => bridge.Configure(input));
    }

    public AppCommandResult Start()
    {
        return ExecuteCommand(bridge.Start);
    }

    public AppCommandResult Pause()
    {
        return ExecuteCommand(bridge.Pause);
    }

    public AppCommandResult Resume()
    {
        return ExecuteCommand(bridge.Resume);
    }

    public AppCommandResult Reset()
    {
        return ExecuteCommand(bridge.Reset);
    }

    public AppCommandResult Tick(int elapsedSeconds)
    {
        return ExecuteCommand(() => bridge.Tick(elapsedSeconds));
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            var shutdownResult = TryStopAudioForShutdown();
            currentState = currentState with
            {
                LastAudioResult = shutdownResult
            };
        }
    }

    private AppCommandResult ExecuteCommand(Func<TimerBridgeUpdate> command)
    {
        lock (_sync)
        {
            if (disposed)
            {
                return BuildTechnicalFailureResult(currentState.CurrentModel, currentState.LastAudioResult);
            }

            TimerBridgeUpdate update;
            try
            {
                update = command();
            }
            catch (Exception)
            {
                return BuildTechnicalFailureResult(currentState.CurrentModel, currentState.LastAudioResult);
            }

            var dispatchResult = systemActionDispatcher.Dispatch(update.SystemActions);
            var result = new AppCommandResult(
                update.DisplayModel,
                dispatchResult.LastAudioResult,
                Success: true,
                dispatchResult.HasWarning,
                dispatchResult.HasTechnicalError,
                dispatchResult.IgnoredActions);

            currentState = new TimerAppState(
                update.DisplayModel,
                dispatchResult.LastAudioResult,
                result);

            return result;
        }
    }

    private AppCommandResult BuildTechnicalFailureResult(
        TimerDisplayModel currentModel,
        AudioServiceResult? lastAudioResult)
    {
        var result = new AppCommandResult(
            currentModel,
            lastAudioResult,
            Success: false,
            HasAudioWarning: false,
            HasTechnicalError: true,
            UnhandledActionCount: 0);

        currentState = currentState with
        {
            LastCommandResult = result
        };

        return result;
    }

    private AudioServiceResult TryStopAudioForShutdown()
    {
        try
        {
            return systemActionDispatcher.Dispatch(
                new[] { SystemActionRequest.StopFinalAlertSound }).LastAudioResult
                ?? new AudioServiceResult(
                    AudioActionResult.NotAttempted,
                    AudioActionResult.NotAttempted,
                    AudioActionResult.NotAttempted);
        }
        catch (Exception)
        {
            return new AudioServiceResult(
                AudioActionResult.PlaybackFailed,
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted);
        }
    }
}

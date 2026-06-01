namespace CicloTimer.Core.Timer;

public sealed class TimerEngine
{
    private TimerConfiguration? _configuration;
    private int _remainingSeconds;

    public TimerState CurrentState { get; private set; } = TimerState.Stopped;
    public int RemainingSeconds => _remainingSeconds;
    public int CompletedSessions { get; private set; }
    public bool IsConfigured => _configuration is not null;

    public bool IsFinalAlertActive => CurrentState == TimerState.FinalAlert;

    public bool CanStart => IsConfigured && CurrentState == TimerState.Stopped;
    public bool CanPause => CurrentState is TimerState.Running or TimerState.FinalAlert;
    public bool CanResume => CurrentState == TimerState.Paused;
    public bool CanReset => IsConfigured;

    public TimerCommandResult ConfigureTimer(TimerConfiguration configuration)
    {
        var validationError = ValidateConfiguration(configuration);
        if (validationError is not null)
        {
            return CreateResult(
                success: false,
                errors: [validationError.Value],
                events: [TimerEvent.ValidationFailed]);
        }

        _configuration = configuration;
        _remainingSeconds = configuration.SessionDurationSeconds;
        CurrentState = TimerState.Stopped;

        return CreateResult(
            success: true,
            events: [TimerEvent.TimerConfigured]);
    }

    public TimerCommandResult StartTimer()
    {
        if (!IsConfigured)
        {
            return CreateResult(
                success: false,
                errors: [TimerError.TimerNotConfigured],
                events: [TimerEvent.ValidationFailed]);
        }

        if (CurrentState != TimerState.Stopped)
        {
            return CreateResult(
                success: false,
                errors: [TimerError.CannotStart],
                events: [TimerEvent.ValidationFailed]);
        }

        CurrentState = TimerState.Running;
        _remainingSeconds = _configuration!.SessionDurationSeconds;

        return CreateResult(
            success: true,
            events: [TimerEvent.TimerStarted]);
    }

    public TimerCommandResult PauseTimer()
    {
        if (CurrentState is not (TimerState.Running or TimerState.FinalAlert))
        {
            return CreateResult(
                success: false,
                errors: [TimerError.CannotPause],
                events: [TimerEvent.ValidationFailed]);
        }

        CurrentState = TimerState.Paused;

        return CreateResult(
            success: true,
            events: [TimerEvent.TimerPaused]);
    }

    public TimerCommandResult ResumeTimer()
    {
        if (CurrentState != TimerState.Paused)
        {
            return CreateResult(
                success: false,
                errors: [TimerError.CannotResume],
                events: [TimerEvent.ValidationFailed]);
        }

        var config = _configuration!;
        var events = new List<TimerEvent> { TimerEvent.TimerResumed };

        if (config.FinalAlertDurationSeconds > 0
            && _remainingSeconds > 0
            && _remainingSeconds <= config.FinalAlertDurationSeconds)
        {
            CurrentState = TimerState.FinalAlert;
            events.Add(TimerEvent.FinalAlertStarted);
        }
        else
        {
            CurrentState = TimerState.Running;
        }

        return CreateResult(success: true, events: events);
    }

    public TimerCommandResult ResetTimer()
    {
        if (!IsConfigured)
        {
            return CreateResult(
                success: false,
                errors: [TimerError.TimerNotConfigured],
                events: [TimerEvent.ValidationFailed]);
        }

        CurrentState = TimerState.Stopped;
        _remainingSeconds = _configuration!.SessionDurationSeconds;

        return CreateResult(
            success: true,
            events: [TimerEvent.TimerReset]);
    }

    public TimerCommandResult Tick(int elapsedSeconds)
    {
        if (elapsedSeconds <= 0)
        {
            return CreateResult(
                success: false,
                errors: [TimerError.InvalidTickDuration]);
        }

        if (CurrentState is TimerState.Stopped or TimerState.Paused)
        {
            return CreateResult(success: true);
        }

        var config = _configuration!;
        var events = new List<TimerEvent>();
        var wasInFinalAlert = CurrentState == TimerState.FinalAlert;

        _remainingSeconds = Math.Max(0, _remainingSeconds - elapsedSeconds);

        if (_remainingSeconds == 0)
        {
            CompleteSessionAndRestart(events);
        }
        else if (!wasInFinalAlert
                 && config.FinalAlertDurationSeconds > 0
                 && _remainingSeconds <= config.FinalAlertDurationSeconds)
        {
            CurrentState = TimerState.FinalAlert;
            events.Add(TimerEvent.FinalAlertStarted);
        }
        else if (CurrentState != TimerState.FinalAlert)
        {
            CurrentState = TimerState.Running;
        }

        return CreateResult(success: true, events: events);
    }

    private void CompleteSessionAndRestart(List<TimerEvent> events)
    {
        events.Add(TimerEvent.SessionCompleted);

        CompletedSessions++;
        events.Add(TimerEvent.SessionCounterIncremented);

        _remainingSeconds = _configuration!.SessionDurationSeconds;
        CurrentState = TimerState.Running;
        events.Add(TimerEvent.NextSessionStarted);
    }

    private static TimerError? ValidateConfiguration(TimerConfiguration configuration)
    {
        if (configuration.SessionDurationSeconds <= 0)
        {
            return TimerError.InvalidSessionDuration;
        }

        if (configuration.FinalAlertDurationSeconds < 0)
        {
            return TimerError.InvalidFinalAlertDuration;
        }

        if (configuration.FinalAlertDurationSeconds >= configuration.SessionDurationSeconds)
        {
            return TimerError.FinalAlertNotLessThanSessionDuration;
        }

        return null;
    }

    private TimerCommandResult CreateResult(
        bool success,
        IReadOnlyList<TimerError>? errors = null,
        IReadOnlyList<TimerEvent>? events = null)
    {
        return new TimerCommandResult
        {
            Success = success,
            State = CurrentState,
            RemainingSeconds = _remainingSeconds,
            CompletedSessions = CompletedSessions,
            IsConfigured = IsConfigured,
            Errors = errors ?? [],
            Events = events ?? []
        };
    }
}

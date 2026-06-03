using CicloTimer.Core.Timer;
using CicloTimer.Localization;

namespace CicloTimer.Bridge;

public sealed class TimerBridge
{
    private readonly TimerEngine _timerEngine;
    private readonly LocalizationService _localization;
    private readonly TimerStateTextMapper _stateTextMapper;
    private readonly TimerErrorTextMapper _errorTextMapper;
    private readonly TimerEventTextMapper _eventTextMapper;
    private readonly PrimaryActionResolver _primaryActionResolver;
    private readonly SystemActionResolver _systemActionResolver;

    public TimerBridge(TimerEngine? timerEngine = null, LocalizationService? localization = null)
    {
        _timerEngine = timerEngine ?? new TimerEngine();
        _localization = localization ?? new LocalizationService();
        _stateTextMapper = new TimerStateTextMapper(_localization);
        _errorTextMapper = new TimerErrorTextMapper(_localization);
        _eventTextMapper = new TimerEventTextMapper(_localization);
        _primaryActionResolver = new PrimaryActionResolver(_localization);
        _systemActionResolver = new SystemActionResolver();
    }

    public TimerBridgeUpdate Configure(TimerInput input)
    {
        var sessionDurationSeconds = input.SessionMinutes * 60 + input.SessionSeconds;
        var finalAlertDurationSeconds = input.FinalAlertMinutes * 60 + input.FinalAlertSeconds;
        var configuration = new TimerConfiguration
        {
            SessionDurationSeconds = sessionDurationSeconds,
            FinalAlertDurationSeconds = finalAlertDurationSeconds
        };

        return ExecuteCommand(() => _timerEngine.ConfigureTimer(configuration));
    }

    public TimerBridgeUpdate Start()
    {
        return ExecuteCommand(_timerEngine.StartTimer);
    }

    public TimerBridgeUpdate Pause()
    {
        return ExecuteCommand(_timerEngine.PauseTimer);
    }

    public TimerBridgeUpdate Resume()
    {
        return ExecuteCommand(_timerEngine.ResumeTimer);
    }

    public TimerBridgeUpdate Reset()
    {
        return ExecuteCommand(_timerEngine.ResetTimer);
    }

    public TimerBridgeUpdate Tick(int elapsedSeconds)
    {
        return ExecuteCommand(() => _timerEngine.Tick(elapsedSeconds));
    }

    public TimerBridgeUpdate GetCurrentUpdate()
    {
        var displayModel = BuildDisplayModel(
            errors: Array.Empty<TimerError>(),
            events: Array.Empty<TimerEvent>());

        return new TimerBridgeUpdate(displayModel, Array.Empty<SystemActionRequest>());
    }

    private TimerBridgeUpdate ExecuteCommand(Func<TimerCommandResult> command)
    {
        var wasFinalAlertActive = _timerEngine.IsFinalAlertActive;
        var result = command();
        var displayModel = BuildDisplayModel(result.Errors, result.Events);
        var systemActions = _systemActionResolver.Resolve(
            result.Events,
            wasFinalAlertActive,
            _timerEngine.IsFinalAlertActive);

        return new TimerBridgeUpdate(displayModel, systemActions);
    }

    private TimerDisplayModel BuildDisplayModel(
        IReadOnlyList<TimerError> errors,
        IReadOnlyList<TimerEvent> events)
    {
        var remainingTimeText = TimeFormatter.Format(_timerEngine.RemainingSeconds);
        var timerStateText = _stateTextMapper.Map(_timerEngine.CurrentState);
        var completedSessionsText = BuildCompletedSessionsText(_timerEngine.CompletedSessions);
        var primaryActionText = _primaryActionResolver.Resolve(
            _timerEngine.CurrentState,
            _timerEngine.CanStart,
            _timerEngine.CanPause,
            _timerEngine.CanResume,
            _timerEngine.CanReset);
        var errorMessageText = _errorTextMapper.Map(errors);
        var eventMessageText = _eventTextMapper.Map(events, _timerEngine.CompletedSessions);
        var accessibleStatusText = _localization.GetAccessibilityText(
            AccessibilityTextKey.StatusTemplate,
            null,
            remainingTimeText,
            timerStateText,
            completedSessionsText);
        var accessibleEventText = string.IsNullOrEmpty(eventMessageText)
            ? string.Empty
            : eventMessageText;

        return new TimerDisplayModel(
            remainingTimeText,
            timerStateText,
            completedSessionsText,
            primaryActionText,
            _timerEngine.CanStart,
            _timerEngine.CanPause,
            _timerEngine.CanResume,
            _timerEngine.CanReset,
            _timerEngine.IsConfigured,
            _timerEngine.IsFinalAlertActive,
            errorMessageText,
            eventMessageText,
            accessibleStatusText,
            accessibleEventText);
    }

    private string BuildCompletedSessionsText(int completedSessions)
    {
        var completedSessionsLabel = _localization.GetUiText(UiTextKey.CompletedSessions);
        return $"{completedSessionsLabel}: {completedSessions}";
    }
}

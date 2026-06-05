using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CicloTimer.App;
using CicloTimer.App.Timing;
using CicloTimer.Bridge;
using CicloTimer.Localization;

namespace CicloTimer.ViewModels;

public sealed class MainTimerViewModel : INotifyPropertyChanged, IDisposable
{
    private const int DefaultCycleMinutes = 25;
    private const int DefaultCycleSeconds = 0;
    private const int DefaultFinalAlertSeconds = 10;
    private const int MaximumCycleMinutes = 999;
    private const int MaximumSeconds = 59;
    private const int MaximumFinalAlertSeconds = 60;

    private readonly ITimerAppOrchestrator orchestrator;
    private readonly IRealtimeTimerRunner runner;
    private readonly LocalizationService localization;
    private readonly IUiDispatcher uiDispatcher;
    private readonly RelayCommand primaryCommand;
    private readonly RelayCommand resetCommand;

    private int cycleMinutes = DefaultCycleMinutes;
    private int cycleSeconds = DefaultCycleSeconds;
    private int finalAlertSeconds = DefaultFinalAlertSeconds;
    private bool isConfigurationValid = true;
    private string validationErrorText = string.Empty;
    private string remainingTimeText = string.Empty;
    private string timerStateText = string.Empty;
    private string completedSessionsText = string.Empty;
    private string timerAccessibilitySummary = string.Empty;
    private string primaryButtonText = string.Empty;
    private string eventMessageText = string.Empty;
    private bool canStart;
    private bool canPause;
    private bool canResume;
    private bool canReset;
    private bool isDisposed;

    public MainTimerViewModel(
        ITimerAppOrchestrator orchestrator,
        IRealtimeTimerRunner runner,
        LocalizationService localization,
        IUiDispatcher uiDispatcher)
    {
        this.orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
        this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.uiDispatcher = uiDispatcher ?? throw new ArgumentNullException(nameof(uiDispatcher));

        primaryCommand = new RelayCommand(ExecutePrimaryCommand, CanRunPrimaryCommand);
        resetCommand = new RelayCommand(ExecuteResetCommand, () => !isDisposed && canReset);

        AppTitle = this.localization.GetUiText(UiTextKey.AppTitle);
        SessionDurationLabel = this.localization.GetUiText(UiTextKey.SessionDuration);
        FinalAlertDurationLabel = this.localization.GetUiText(UiTextKey.FinalAlertDuration);
        MinutesLabel = this.localization.GetUiText(UiTextKey.Minutes);
        SecondsLabel = this.localization.GetUiText(UiTextKey.Seconds);
        RemainingTimeLabel = this.localization.GetUiText(UiTextKey.RemainingTime);
        TimerStateLabel = this.localization.GetUiText(UiTextKey.TimerState);
        CompletedSessionsLabel = this.localization.GetUiText(UiTextKey.CompletedSessions);
        MessageLabel = this.localization.GetUiText(UiTextKey.Message);
        ResetButtonText = this.localization.GetCommandText(CommandTextKey.Reset);
        PrimaryButtonText = this.localization.GetCommandText(CommandTextKey.Start);

        SessionDurationMinutesAccessibleName = this.localization.GetAccessibilityText(AccessibilityTextKey.SessionDurationMinutes);
        SessionDurationSecondsAccessibleName = this.localization.GetAccessibilityText(AccessibilityTextKey.SessionDurationSeconds);
        FinalAlertDurationSecondsAccessibleName = this.localization.GetAccessibilityText(AccessibilityTextKey.FinalAlertDurationSeconds);

        this.orchestrator.StateChanged += OnOrchestratorStateChanged;
        ValidateConfiguration();
        ConfigureCurrentValues();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string AppTitle { get; }
    public string SessionDurationLabel { get; }
    public string FinalAlertDurationLabel { get; }
    public string MinutesLabel { get; }
    public string SecondsLabel { get; }
    public string RemainingTimeLabel { get; }
    public string TimerStateLabel { get; }
    public string CompletedSessionsLabel { get; }
    public string MessageLabel { get; }
    public string ResetButtonText { get; }
    public string SessionDurationMinutesAccessibleName { get; }
    public string SessionDurationSecondsAccessibleName { get; }
    public string FinalAlertDurationSecondsAccessibleName { get; }

    public ICommand PrimaryCommand => primaryCommand;
    public ICommand ResetCommand => resetCommand;

    public int CycleMinutes
    {
        get => cycleMinutes;
        set
        {
            var clamped = Clamp(value, 0, MaximumCycleMinutes);
            if (SetProperty(ref cycleMinutes, clamped))
            {
                OnConfigurationChanged();
            }
        }
    }

    public int CycleSeconds
    {
        get => cycleSeconds;
        set
        {
            var clamped = Clamp(value, 0, MaximumSeconds);
            if (SetProperty(ref cycleSeconds, clamped))
            {
                OnConfigurationChanged();
            }
        }
    }

    public int FinalAlertSeconds
    {
        get => finalAlertSeconds;
        set
        {
            var clamped = Clamp(value, 0, MaximumFinalAlertSeconds);
            if (SetProperty(ref finalAlertSeconds, clamped))
            {
                OnConfigurationChanged();
            }
        }
    }

    public bool IsConfigurationValid
    {
        get => isConfigurationValid;
        private set => SetProperty(ref isConfigurationValid, value);
    }

    public string ValidationErrorText
    {
        get => validationErrorText;
        private set
        {
            if (SetProperty(ref validationErrorText, value))
            {
                OnPropertyChanged(nameof(HasValidationError));
            }
        }
    }

    public bool HasValidationError => !string.IsNullOrWhiteSpace(ValidationErrorText);

    public string RemainingTimeText
    {
        get => remainingTimeText;
        private set
        {
            if (SetProperty(ref remainingTimeText, value))
            {
                RefreshTimerAccessibilitySummary();
            }
        }
    }

    public string TimerStateText
    {
        get => timerStateText;
        private set
        {
            if (SetProperty(ref timerStateText, value))
            {
                RefreshTimerAccessibilitySummary();
            }
        }
    }

    public string CompletedSessionsText
    {
        get => completedSessionsText;
        private set
        {
            if (SetProperty(ref completedSessionsText, value))
            {
                RefreshTimerAccessibilitySummary();
            }
        }
    }

    public string TimerAccessibilitySummary
    {
        get => timerAccessibilitySummary;
        private set => SetProperty(ref timerAccessibilitySummary, value);
    }

    public string PrimaryButtonText
    {
        get => primaryButtonText;
        private set => SetProperty(ref primaryButtonText, value);
    }

    public string EventMessageText
    {
        get => eventMessageText;
        private set
        {
            if (SetProperty(ref eventMessageText, value))
            {
                OnPropertyChanged(nameof(HasEventMessage));
            }
        }
    }

    public bool HasEventMessage => !string.IsNullOrWhiteSpace(EventMessageText);

    public bool CanExecutePrimaryCommand => CanRunPrimaryCommand();

    public bool CanExecuteResetCommand => !isDisposed && canReset;

    public bool CanEditConfiguration => !canPause && !canResume;

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;
        orchestrator.StateChanged -= OnOrchestratorStateChanged;
        runner.Stop();
        runner.Dispose();
        orchestrator.Dispose();
        RefreshCommandState();
    }

    private void OnConfigurationChanged()
    {
        ValidateConfiguration();

        if (CanEditConfiguration && IsConfigurationValid)
        {
            ConfigureCurrentValues();
        }

        RefreshCommandState();
    }

    private void ExecutePrimaryCommand()
    {
        if (isDisposed)
        {
            return;
        }

        if (canPause)
        {
            Pause();
            return;
        }

        if (canResume)
        {
            Resume();
            return;
        }

        Start();
    }

    private bool CanRunPrimaryCommand()
    {
        if (isDisposed)
        {
            return false;
        }

        if (canPause || canResume)
        {
            return true;
        }

        return IsConfigurationValid && canStart;
    }

    private void ExecuteResetCommand()
    {
        if (isDisposed || !canReset)
        {
            return;
        }

        runner.Stop();
        ApplyResult(orchestrator.Reset());
        EventMessageText = localization.GetTimerText(TimerTextKey.EventTimerReset);
    }

    private void Start()
    {
        ValidateConfiguration();
        if (!IsConfigurationValid)
        {
            RefreshCommandState();
            return;
        }

        var configureResult = orchestrator.Configure(BuildTimerInput());
        ApplyResult(configureResult);
        if (!string.IsNullOrWhiteSpace(configureResult.CurrentModel.ErrorMessageText)
            || configureResult.HasTechnicalError)
        {
            ValidationErrorText = configureResult.CurrentModel.ErrorMessageText;
            RefreshCommandState();
            return;
        }

        var startResult = orchestrator.Start();
        ApplyResult(startResult);
        if (!string.IsNullOrWhiteSpace(startResult.CurrentModel.ErrorMessageText)
            || startResult.HasTechnicalError)
        {
            ValidationErrorText = startResult.CurrentModel.ErrorMessageText;
            RefreshCommandState();
            return;
        }

        EventMessageText = localization.GetTimerText(TimerTextKey.EventTimerStarted);
        runner.Start();
    }

    private void Pause()
    {
        var result = orchestrator.Pause();
        ApplyResult(result);
        if (string.IsNullOrWhiteSpace(result.CurrentModel.ErrorMessageText)
            && !result.HasTechnicalError)
        {
            EventMessageText = localization.GetTimerText(TimerTextKey.EventTimerPaused);
            runner.Stop();
        }
    }

    private void Resume()
    {
        var result = orchestrator.Resume();
        ApplyResult(result);
        if (string.IsNullOrWhiteSpace(result.CurrentModel.ErrorMessageText)
            && !result.HasTechnicalError)
        {
            EventMessageText = localization.GetTimerText(TimerTextKey.EventTimerResumed);
            runner.Start();
        }
    }

    private void ConfigureCurrentValues()
    {
        if (isDisposed)
        {
            return;
        }

        ApplyResult(orchestrator.Configure(BuildTimerInput()));
    }

    private TimerInput BuildTimerInput()
    {
        return new TimerInput(CycleMinutes, CycleSeconds, 0, FinalAlertSeconds);
    }

    private void OnOrchestratorStateChanged(object? sender, TimerAppStateChangedEventArgs args)
    {
        uiDispatcher.Post(() => ApplyState(args.State.CurrentModel));
    }

    private void ApplyResult(AppCommandResult result)
    {
        uiDispatcher.Post(() => ApplyState(result.CurrentModel));
    }

    private void ApplyState(TimerDisplayModel model)
    {
        RemainingTimeText = model.RemainingTimeText;
        TimerStateText = model.TimerStateText;
        CompletedSessionsText = model.CompletedSessionsText;
        PrimaryButtonText = string.IsNullOrWhiteSpace(model.PrimaryActionText)
            ? localization.GetCommandText(CommandTextKey.Start)
            : model.PrimaryActionText;

        canStart = model.CanStart;
        canPause = model.CanPause;
        canResume = model.CanResume;
        canReset = model.CanReset;

        if (!string.IsNullOrWhiteSpace(model.ErrorMessageText))
        {
            ValidationErrorText = model.ErrorMessageText;
            IsConfigurationValid = false;
        }
        else
        {
            ValidateConfiguration();
        }

        OnPropertyChanged(nameof(CanExecutePrimaryCommand));
        OnPropertyChanged(nameof(CanExecuteResetCommand));
        OnPropertyChanged(nameof(CanEditConfiguration));
        RefreshCommandState();
    }

    private void ValidateConfiguration()
    {
        var totalSeconds = (CycleMinutes * 60) + CycleSeconds;

        if (totalSeconds <= 0)
        {
            IsConfigurationValid = false;
            ValidationErrorText = localization.GetErrorText(ErrorTextKey.InvalidSessionDuration);
            return;
        }

        if (FinalAlertSeconds >= totalSeconds)
        {
            IsConfigurationValid = false;
            ValidationErrorText = localization.GetErrorText(ErrorTextKey.FinalAlertNotLessThanSessionDuration);
            return;
        }

        IsConfigurationValid = true;
        ValidationErrorText = string.Empty;
    }

    private void RefreshCommandState()
    {
        primaryCommand.RaiseCanExecuteChanged();
        resetCommand.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(CanExecutePrimaryCommand));
        OnPropertyChanged(nameof(CanExecuteResetCommand));
        OnPropertyChanged(nameof(CanEditConfiguration));
    }

    private void RefreshTimerAccessibilitySummary()
    {
        TimerAccessibilitySummary = localization.GetAccessibilityText(
            AccessibilityTextKey.StatusTemplate,
            null,
            RemainingTimeText,
            TimerStateText,
            CompletedSessionsText);
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static int Clamp(int value, int minimum, int maximum)
    {
        return Math.Min(Math.Max(value, minimum), maximum);
    }
}

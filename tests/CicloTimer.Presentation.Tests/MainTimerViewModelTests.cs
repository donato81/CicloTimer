using CicloTimer.Localization;
using CicloTimer.ViewModels;

namespace CicloTimer.Presentation.Tests;

public sealed class MainTimerViewModelTests
{
    [Fact]
    public void ConstructorConfiguresDefaultTimerAndSubscribesToStateChanged()
    {
        var viewModel = CreateViewModel(out var orchestrator, out _);

        Assert.Equal(1, orchestrator.StateChangedAddCount);
        Assert.Equal(1, orchestrator.ConfigureCalls);
        Assert.NotNull(orchestrator.LastInput);
        Assert.Equal(25, orchestrator.LastInput!.SessionMinutes);
        Assert.Equal(0, orchestrator.LastInput.SessionSeconds);
        Assert.Equal(0, orchestrator.LastInput.FinalAlertMinutes);
        Assert.Equal(10, orchestrator.LastInput.FinalAlertSeconds);
        Assert.Equal("25:00", viewModel.RemainingTimeText);
        Assert.True(viewModel.IsConfigurationValid);
        Assert.True(viewModel.PrimaryCommand.CanExecute(null));
    }

    [Fact]
    public void DurationZeroDisablesPrimaryCommandAndShowsLocalizedError()
    {
        var viewModel = CreateViewModel(out _, out _);

        viewModel.CycleMinutes = 0;
        viewModel.CycleSeconds = 0;

        Assert.False(viewModel.IsConfigurationValid);
        Assert.False(viewModel.PrimaryCommand.CanExecute(null));
        Assert.Equal(
            "La durata della sessione deve essere maggiore di zero.",
            viewModel.ValidationErrorText);
    }

    [Fact]
    public void FinalAlertEqualToDurationDisablesPrimaryCommand()
    {
        var viewModel = CreateViewModel(out _, out _);

        viewModel.CycleMinutes = 0;
        viewModel.CycleSeconds = 10;
        viewModel.FinalAlertSeconds = 10;

        Assert.False(viewModel.IsConfigurationValid);
        Assert.False(viewModel.PrimaryCommand.CanExecute(null));
        Assert.Equal(
            "La durata dell'avviso finale deve essere inferiore alla durata della sessione.",
            viewModel.ValidationErrorText);
    }

    [Fact]
    public void PrimaryCommandFromStoppedConfiguresStartsAndStartsRunner()
    {
        var viewModel = CreateViewModel(out var orchestrator, out var runner);

        viewModel.PrimaryCommand.Execute(null);

        Assert.Equal(2, orchestrator.ConfigureCalls);
        Assert.Equal(1, orchestrator.StartCalls);
        Assert.Equal(1, runner.StartCalls);
        Assert.Equal("Pausa", viewModel.PrimaryButtonText);
        Assert.True(viewModel.CanExecutePrimaryCommand);
    }

    [Fact]
    public void PrimaryCommandFromRunningPausesAndStopsRunner()
    {
        var viewModel = CreateViewModel(out var orchestrator, out var runner);
        viewModel.PrimaryCommand.Execute(null);

        viewModel.PrimaryCommand.Execute(null);

        Assert.Equal(1, orchestrator.PauseCalls);
        Assert.Equal(1, runner.StopCalls);
        Assert.Equal("Riprendi", viewModel.PrimaryButtonText);
    }

    [Fact]
    public void PrimaryCommandFromPausedResumesAndStartsRunner()
    {
        var viewModel = CreateViewModel(out var orchestrator, out var runner);
        viewModel.PrimaryCommand.Execute(null);
        viewModel.PrimaryCommand.Execute(null);

        viewModel.PrimaryCommand.Execute(null);

        Assert.Equal(1, orchestrator.ResumeCalls);
        Assert.Equal(2, runner.StartCalls);
        Assert.Equal("Pausa", viewModel.PrimaryButtonText);
    }

    [Fact]
    public void ResetStopsRunnerAndKeepsConfiguration()
    {
        var viewModel = CreateViewModel(out var orchestrator, out var runner);
        viewModel.CycleMinutes = 3;
        viewModel.CycleSeconds = 15;
        viewModel.FinalAlertSeconds = 5;

        viewModel.ResetCommand.Execute(null);

        Assert.Equal(1, runner.StopCalls);
        Assert.Equal(1, orchestrator.ResetCalls);
        Assert.Equal(3, viewModel.CycleMinutes);
        Assert.Equal(15, viewModel.CycleSeconds);
        Assert.Equal(5, viewModel.FinalAlertSeconds);
    }

    [Fact]
    public void StateChangedUpdatesBoundDisplayProperties()
    {
        var viewModel = CreateViewModel(out var orchestrator, out _);

        orchestrator.RaiseStateChanged(TestDisplayModels.Running("12:34"));

        Assert.Equal("12:34", viewModel.RemainingTimeText);
        Assert.Equal("Sessione in corso", viewModel.TimerStateText);
        Assert.Equal("Pausa", viewModel.PrimaryButtonText);
    }

    [Fact]
    public void DisposeStopsRunnerDisposesDependenciesAndRemovesStateChangedSubscription()
    {
        var viewModel = CreateViewModel(out var orchestrator, out var runner);

        viewModel.Dispose();
        viewModel.Dispose();

        Assert.Equal(1, orchestrator.StateChangedRemoveCount);
        Assert.Equal(1, orchestrator.DisposeCalls);
        Assert.Equal(1, runner.DisposeCalls);
        Assert.True(runner.StopCalls >= 1);
    }

    private static MainTimerViewModel CreateViewModel(
        out FakeTimerAppOrchestrator orchestrator,
        out FakeRealtimeTimerRunner runner)
    {
        orchestrator = new FakeTimerAppOrchestrator();
        runner = new FakeRealtimeTimerRunner();

        return new MainTimerViewModel(
            orchestrator,
            runner,
            new LocalizationService(),
            new ImmediateUiDispatcher());
    }
}

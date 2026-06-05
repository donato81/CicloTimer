using System.Threading;
using System.Windows;
using CicloTimer.App;
using CicloTimer.App.Timing;
using CicloTimer.Audio;
using CicloTimer.Bridge;
using CicloTimer.Localization;
using CicloTimer.ViewModels;

namespace CicloTimer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = CreateViewModel();
    }

    private static MainTimerViewModel CreateViewModel()
    {
        var localization = new LocalizationService();
        var bridge = new TimerBridge(localization: localization);
        var bridgePort = new TimerBridgeAdapter(bridge);
        var audioPort = new AudioServiceAdapter(new AudioService());
        var orchestrator = new TimerAppOrchestrator(bridgePort, audioPort);
        var runner = new RealtimeTimerRunner(orchestrator);
        var uiDispatcher = new SynchronizationContextUiDispatcher(SynchronizationContext.Current);

        return new MainTimerViewModel(orchestrator, runner, localization, uiDispatcher);
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Set initial focus to first interactive control (Session Duration Minutes)
        SessionMinutesControl?.Focus();
    }
}

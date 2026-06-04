using CicloTimer.App;
using CicloTimer.App.Tests;
using CicloTimer.Audio;
using CicloTimer.Bridge;

namespace CicloTimer.App.Tests.Timing.Fakes;

internal sealed class FakeTimerAppOrchestrator : ITimerAppOrchestrator
{
    private static readonly TimeSpan ReleaseTickTimeout = TimeSpan.FromSeconds(2);

    private readonly object _sync = new();
    private int _activeTicks;
    private int _maxConcurrentTicks;
    private bool _disposed;

    public event EventHandler<TimerAppStateChangedEventArgs>? StateChanged
    {
        add { }
        remove { }
    }

    public TimerAppState CurrentState { get; private set; } = new(
        TestModelFactory.Create("runner"),
        LastAudioResult: null,
        LastCommandResult: null);

    public List<int> TickCalls { get; } = new();

    public bool ThrowOnTick { get; set; }

    public ManualResetEventSlim? TickEntered { get; set; }

    public ManualResetEventSlim? ReleaseTick { get; set; }

    public int MaxConcurrentTicks => _maxConcurrentTicks;

    public bool IsDisposed => _disposed;

    public AppCommandResult Configure(TimerInput input)
    {
        return BuildSuccessResult();
    }

    public AppCommandResult Start()
    {
        return BuildSuccessResult();
    }

    public AppCommandResult Pause()
    {
        return BuildSuccessResult();
    }

    public AppCommandResult Resume()
    {
        return BuildSuccessResult();
    }

    public AppCommandResult Reset()
    {
        return BuildSuccessResult();
    }

    public AppCommandResult Tick(int elapsedSeconds)
    {
        var activeTicks = Interlocked.Increment(ref _activeTicks);
        TrackMaxConcurrentTicks(activeTicks);

        try
        {
            TickEntered?.Set();
            if (ReleaseTick is not null && !ReleaseTick.Wait(ReleaseTickTimeout))
            {
                throw new TimeoutException("Timed out waiting for test tick release.");
            }

            if (ThrowOnTick)
            {
                throw new InvalidOperationException("Technical test exception");
            }

            lock (_sync)
            {
                TickCalls.Add(elapsedSeconds);
            }

            return BuildSuccessResult();
        }
        finally
        {
            Interlocked.Decrement(ref _activeTicks);
        }
    }

    public void Dispose()
    {
        _disposed = true;
    }

    public IReadOnlyList<int> SnapshotTickCalls()
    {
        lock (_sync)
        {
            return TickCalls.ToArray();
        }
    }

    private void TrackMaxConcurrentTicks(int activeTicks)
    {
        int observed;
        do
        {
            observed = _maxConcurrentTicks;
            if (activeTicks <= observed)
            {
                return;
            }
        }
        while (Interlocked.CompareExchange(ref _maxConcurrentTicks, activeTicks, observed) != observed);
    }

    private AppCommandResult BuildSuccessResult()
    {
        var result = new AppCommandResult(
            CurrentState.CurrentModel,
            new AudioServiceResult(
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted),
            Success: true,
            HasAudioWarning: false,
            HasTechnicalError: false,
            UnhandledActionCount: 0);

        CurrentState = CurrentState with
        {
            LastCommandResult = result
        };

        return result;
    }
}

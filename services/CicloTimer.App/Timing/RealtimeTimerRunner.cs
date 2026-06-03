namespace CicloTimer.App.Timing;

public sealed class RealtimeTimerRunner : IRealtimeTimerRunner
{
    private static readonly TimeSpan TickPeriod = TimeSpan.FromSeconds(1);

    private readonly object _sync = new();
    private readonly ITimerAppOrchestrator _orchestrator;
    private readonly TimeProvider _timeProvider;

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _loopTask;
    private bool _isRunning;
    private bool _disposed;
    private DateTimeOffset _lastProcessedTime;
    private TimeSpan _remainder;

    public RealtimeTimerRunner(
        ITimerAppOrchestrator orchestrator,
        TimeProvider? timeProvider = null)
    {
        _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public bool IsRunning
    {
        get
        {
            lock (_sync)
            {
                return _isRunning;
            }
        }
    }

    public void Start()
    {
        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_isRunning)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            _lastProcessedTime = _timeProvider.GetUtcNow();
            _remainder = TimeSpan.Zero;
            _isRunning = true;
            _loopTask = RunLoopAsync(cancellationToken);
        }
    }

    public void Stop()
    {
        CancellationTokenSource? cancellationTokenSource;

        lock (_sync)
        {
            if (!_isRunning && _cancellationTokenSource is null)
            {
                return;
            }

            cancellationTokenSource = _cancellationTokenSource;
            _cancellationTokenSource = null;
            _isRunning = false;
        }

        cancellationTokenSource?.Cancel();
    }

    public void Dispose()
    {
        CancellationTokenSource? cancellationTokenSource;

        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _isRunning = false;
            cancellationTokenSource = _cancellationTokenSource;
            _cancellationTokenSource = null;
        }

        if (cancellationTokenSource is null)
        {
            return;
        }

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    private async Task RunLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TickPeriod, _timeProvider);

            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                ProcessElapsedTime(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception)
        {
        }
        finally
        {
            MarkStopped(cancellationToken);
        }
    }

    private void ProcessElapsedTime(CancellationToken cancellationToken)
    {
        int elapsedSeconds;

        lock (_sync)
        {
            if (!_isRunning || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var now = _timeProvider.GetUtcNow();
            var delta = now - _lastProcessedTime;

            if (delta < TimeSpan.Zero)
            {
                _lastProcessedTime = now;
                return;
            }

            var total = _remainder + delta;
            elapsedSeconds = (int)total.TotalSeconds;

            if (elapsedSeconds <= 0)
            {
                _remainder = total;
                _lastProcessedTime = now;
                return;
            }

            _remainder = total - TimeSpan.FromSeconds(elapsedSeconds);
            _lastProcessedTime = now;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _orchestrator.Tick(elapsedSeconds);
    }

    private void MarkStopped(CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            if (_cancellationTokenSource is null)
            {
                _isRunning = false;
                return;
            }

            if (_cancellationTokenSource.Token == cancellationToken)
            {
                _cancellationTokenSource = null;
                _isRunning = false;
            }
        }
    }
}

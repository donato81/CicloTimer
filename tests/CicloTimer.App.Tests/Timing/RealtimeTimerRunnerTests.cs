using CicloTimer.App.Tests.Timing.Fakes;
using CicloTimer.App.Timing;
using Microsoft.Extensions.Time.Testing;

namespace CicloTimer.App.Tests;

public sealed class RealtimeTimerRunnerTests
{
    [Fact]
    public void ConstructorRequiresOrchestrator()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RealtimeTimerRunner(null!));
    }

    [Fact]
    public void StartSetsIsRunning()
    {
        using var runner = CreateRunner(out _, out _);

        runner.Start();

        Assert.True(runner.IsRunning);
    }

    [Fact]
    public void StopSetsIsRunningFalse()
    {
        using var runner = CreateRunner(out _, out _);

        runner.Start();
        runner.Stop();

        Assert.False(runner.IsRunning);
    }

    [Fact]
    public void StartCalledTwiceDoesNotCreateSecondLoop()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        runner.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(1));

        Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count == 1));
        Assert.Equal(new[] { 1 }, orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void StopCalledTwiceDoesNotThrow()
    {
        using var runner = CreateRunner(out _, out _);

        runner.Start();
        runner.Stop();
        var exception = Record.Exception(runner.Stop);

        Assert.Null(exception);
    }

    [Fact]
    public void StartStopStartUsesNewCleanRun()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        timeProvider.Advance(TimeSpan.FromMilliseconds(800));
        runner.Stop();

        runner.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(1));

        Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count == 1));
        Assert.Equal(new[] { 1 }, orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void DisposeStopsRunner()
    {
        var runner = CreateRunner(out _, out _);

        runner.Start();
        runner.Dispose();

        Assert.False(runner.IsRunning);
    }

    [Fact]
    public void DisposeCanBeCalledTwice()
    {
        var runner = CreateRunner(out _, out _);

        runner.Dispose();
        var exception = Record.Exception(runner.Dispose);

        Assert.Null(exception);
    }

    [Fact]
    public void StartAfterDisposeThrowsObjectDisposedException()
    {
        var runner = CreateRunner(out _, out _);

        runner.Dispose();

        Assert.Throws<ObjectDisposedException>(runner.Start);
    }

    [Fact]
    public void StopAfterDisposeDoesNotThrow()
    {
        var runner = CreateRunner(out _, out _);

        runner.Dispose();
        var exception = Record.Exception(runner.Stop);

        Assert.Null(exception);
    }

    [Fact]
    public void StartDoesNotProduceImmediateTick()
    {
        using var runner = CreateRunner(out var orchestrator, out _);

        runner.Start();

        Assert.Empty(orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void OneSimulatedSecondProducesTickOne()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(1));

        Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count == 1));
        Assert.Equal(new[] { 1 }, orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void TwoSimulatedSecondsProduceTickTwo()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(2));

        Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count == 1));
        Assert.Equal(new[] { 2 }, orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void LessThanOneSecondDoesNotProduceTickZero()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        timeProvider.Advance(TimeSpan.FromMilliseconds(800));

        Assert.Empty(orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void FractionalRemainderIsPreserved()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        timeProvider.Advance(TimeSpan.FromMilliseconds(1400));
        Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count == 1));

        timeProvider.Advance(TimeSpan.FromMilliseconds(700));
        Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count == 2));

        Assert.Equal(new[] { 1, 1 }, orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void DriftIsCalculatedFromElapsedTimeNotWakeCount()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        timeProvider.Advance(TimeSpan.FromMilliseconds(3500));

        Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count == 1));
        Assert.Equal(new[] { 3 }, orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void NoTickOccursAfterStop()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        runner.Stop();
        timeProvider.Advance(TimeSpan.FromSeconds(2));

        Assert.Empty(orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void NoTickOccursAfterDispose()
    {
        var runner = CreateRunner(out var orchestrator, out var timeProvider);

        runner.Start();
        runner.Dispose();
        timeProvider.Advance(TimeSpan.FromSeconds(2));

        Assert.Empty(orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void TickExceptionStopsRunner()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);
        orchestrator.ThrowOnTick = true;

        runner.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(1));

        Assert.True(WaitUntil(() => !runner.IsRunning));
        Assert.Empty(orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public void TickExceptionDoesNotLeaveLoopZombie()
    {
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);
        orchestrator.ThrowOnTick = true;

        runner.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        Assert.True(WaitUntil(() => !runner.IsRunning));

        orchestrator.ThrowOnTick = false;
        timeProvider.Advance(TimeSpan.FromSeconds(3));

        Assert.Empty(orchestrator.SnapshotTickCalls());
    }

    [Fact]
    public async Task TickCallsAreNotParallel()
    {
        var timeout = TimeSpan.FromMilliseconds(500);
        var observationTimeout = TimeSpan.FromMilliseconds(50);
        using var runner = CreateRunner(out var orchestrator, out var timeProvider);
        using var tickEntered = new ManualResetEventSlim();
        using var releaseTick = new ManualResetEventSlim();
        orchestrator.TickEntered = tickEntered;
        orchestrator.ReleaseTick = releaseTick;
        Task? firstAdvance = null;
        Task? secondAdvance = null;

        try
        {
            runner.Start();
            firstAdvance = Task.Run(() => timeProvider.Advance(TimeSpan.FromSeconds(1)));

            Assert.True(tickEntered.Wait(timeout));

            secondAdvance = Task.Run(() => timeProvider.Advance(TimeSpan.FromSeconds(3)));
            try
            {
                await secondAdvance.WaitAsync(observationTimeout);
            }
            catch (TimeoutException)
            {
            }

            Assert.Equal(1, orchestrator.MaxConcurrentTicks);
            Assert.Empty(orchestrator.SnapshotTickCalls());

            releaseTick.Set();

            await firstAdvance.WaitAsync(timeout);
            await secondAdvance.WaitAsync(timeout);
            Assert.True(WaitUntil(() => orchestrator.SnapshotTickCalls().Count >= 1));
            Assert.Equal(1, orchestrator.MaxConcurrentTicks);
        }
        finally
        {
            releaseTick.Set();
            if (firstAdvance is not null)
            {
                await firstAdvance.WaitAsync(timeout);
            }

            if (secondAdvance is not null)
            {
                await secondAdvance.WaitAsync(timeout);
            }
        }
    }

    [Fact]
    public void TimingSourceDoesNotContainForbiddenReferences()
    {
        var timingDirectory = Path.Combine(ProjectPaths.RepositoryRoot, "services", "CicloTimer.App", "Timing");
        var source = string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(timingDirectory, "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(File.ReadAllText));

        string[] forbidden =
        [
            "DispatcherTimer",
            "System.Windows.Threading",
            "System.Windows",
            "Dispatcher",
            "CicloTimer.Core",
            "CicloTimer.Bridge",
            "CicloTimer.Audio",
            "CicloTimer.Localization",
            "TimerBridgeAdapter",
            "SystemActionDispatcher",
            "Task.Delay",
            "System.Timers.Timer",
            "System.Threading.Timer"
        ];

        foreach (var forbiddenText in forbidden)
        {
            Assert.DoesNotContain(forbiddenText, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void RunnerUsesTimeProviderAndTimeSpanState()
    {
        var fields = typeof(RealtimeTimerRunner).GetFields(
            System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.NonPublic);

        Assert.Contains(fields, field => field.FieldType == typeof(TimeProvider));
        Assert.Contains(fields, field => field.FieldType == typeof(TimeSpan));
        Assert.DoesNotContain(fields, field => field.FieldType == typeof(double) || field.FieldType == typeof(float));
    }

    private static RealtimeTimerRunner CreateRunner(
        out FakeTimerAppOrchestrator orchestrator,
        out FakeTimeProvider timeProvider)
    {
        orchestrator = new FakeTimerAppOrchestrator();
        timeProvider = new FakeTimeProvider();
        return new RealtimeTimerRunner(orchestrator, timeProvider);
    }

    private static bool WaitUntil(Func<bool> condition)
    {
        return SpinWait.SpinUntil(condition, TimeSpan.FromMilliseconds(500));
    }
}

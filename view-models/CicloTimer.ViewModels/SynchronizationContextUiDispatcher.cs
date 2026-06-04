using System.Threading;

namespace CicloTimer.ViewModels;

public sealed class SynchronizationContextUiDispatcher : IUiDispatcher
{
    private readonly SynchronizationContext? synchronizationContext;

    public SynchronizationContextUiDispatcher(SynchronizationContext? synchronizationContext)
    {
        this.synchronizationContext = synchronizationContext;
    }

    public void Post(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (synchronizationContext is null)
        {
            action();
            return;
        }

        synchronizationContext.Post(static state => ((Action)state!)(), action);
    }
}

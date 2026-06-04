namespace CicloTimer.ViewModels;

public interface IUiDispatcher
{
    void Post(Action action);
}

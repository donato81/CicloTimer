using Xunit;

namespace CicloTimer.Bridge.Tests;

public sealed class TimerBridgeUpdateTests
{
    [Fact]
    public void BridgeCommands_ReturnTimerBridgeUpdate()
    {
        var bridge = new TimerBridge();

        AssertUpdate(bridge.GetCurrentUpdate());
        AssertUpdate(bridge.Configure(new TimerInput(5, 0, 0, 20)));
        AssertUpdate(bridge.Start());
        AssertUpdate(bridge.Pause());
        AssertUpdate(bridge.Resume());
        AssertUpdate(bridge.Tick(1));
        AssertUpdate(bridge.Reset());
    }

    [Fact]
    public void GetCurrentUpdate_ReturnsEmptyEventTextsAndEmptyActions()
    {
        var bridge = new TimerBridge();
        bridge.Configure(new TimerInput(5, 0, 0, 20));

        var update = bridge.GetCurrentUpdate();

        Assert.NotNull(update.DisplayModel);
        Assert.NotNull(update.SystemActions);
        Assert.Empty(update.SystemActions);
        Assert.Equal(string.Empty, update.DisplayModel.EventMessageText);
        Assert.Equal(string.Empty, update.DisplayModel.AccessibleEventText);
        Assert.False(string.IsNullOrWhiteSpace(update.DisplayModel.AccessibleStatusText));
    }

    private static void AssertUpdate(TimerBridgeUpdate update)
    {
        Assert.NotNull(update);
        Assert.NotNull(update.DisplayModel);
        Assert.NotNull(update.SystemActions);
        Assert.False(string.IsNullOrWhiteSpace(update.DisplayModel.AccessibleStatusText));
    }
}

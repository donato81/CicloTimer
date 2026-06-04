using System.Windows;
using CicloTimer.Views.Controls;

namespace CicloTimer.Presentation.Tests;

public sealed class NumericStepControlTests
{
    [Fact]
    public void ValueDependencyPropertyBindsTwoWayByDefault()
    {
        var metadata = Assert.IsType<FrameworkPropertyMetadata>(
            NumericStepControl.ValueProperty.GetMetadata(typeof(NumericStepControl)));

        Assert.True(metadata.BindsTwoWayByDefault);
    }

    [Fact]
    public void NumericPropertiesAreDependencyProperties()
    {
        Assert.NotNull(NumericStepControl.ValueProperty);
        Assert.NotNull(NumericStepControl.MinimumProperty);
        Assert.NotNull(NumericStepControl.MaximumProperty);
        Assert.NotNull(NumericStepControl.StepProperty);
        Assert.NotNull(NumericStepControl.LabelProperty);
    }

}

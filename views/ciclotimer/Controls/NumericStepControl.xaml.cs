using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CicloTimer.Views.Controls;

public partial class NumericStepControl : UserControl
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(int),
            typeof(NumericStepControl),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged,
                CoerceValue),
            IsValidNumber);

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(int),
            typeof(NumericStepControl),
            new PropertyMetadata(0, OnLimitChanged),
            IsValidNumber);

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(NumericStepControl),
            new PropertyMetadata(100, OnLimitChanged),
            IsValidNumber);

    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(
            nameof(Step),
            typeof(int),
            typeof(NumericStepControl),
            new PropertyMetadata(1),
            value => value is int step && step > 0);

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(NumericStepControl),
            new PropertyMetadata(string.Empty));

    public NumericStepControl()
    {
        InitializeComponent();
    }

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var control = (NumericStepControl)dependencyObject;
        var clamped = control.Clamp((int)args.NewValue);
        if (clamped != (int)args.NewValue)
        {
            control.SetCurrentValue(ValueProperty, clamped);
        }
    }

    private static void OnLimitChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        dependencyObject.CoerceValue(ValueProperty);
    }

    private static object CoerceValue(DependencyObject dependencyObject, object baseValue)
    {
        var control = (NumericStepControl)dependencyObject;
        return control.Clamp((int)baseValue);
    }

    private static bool IsValidNumber(object value)
    {
        return value is int;
    }

    private void DecreaseButton_Click(object sender, RoutedEventArgs e)
    {
        SetCurrentValue(ValueProperty, Clamp(Value - Step));
        BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
    }

    private void IncreaseButton_Click(object sender, RoutedEventArgs e)
    {
        SetCurrentValue(ValueProperty, Clamp(Value + Step));
        BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
    }

    private int Clamp(int value)
    {
        var minimum = Minimum;
        var maximum = Math.Max(minimum, Maximum);
        return Math.Min(Math.Max(value, minimum), maximum);
    }
}

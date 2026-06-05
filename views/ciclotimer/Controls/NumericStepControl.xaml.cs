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

    public static readonly DependencyProperty AccessibleNameProperty =
        DependencyProperty.Register(
            nameof(AccessibleName),
            typeof(string),
            typeof(NumericStepControl),
            new PropertyMetadata(string.Empty, OnAccessibleNameChanged));

    public static readonly DependencyProperty IncreaseAccessibleNameProperty =
        DependencyProperty.Register(
            nameof(IncreaseAccessibleName),
            typeof(string),
            typeof(NumericStepControl),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DecreaseAccessibleNameProperty =
        DependencyProperty.Register(
            nameof(DecreaseAccessibleName),
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

    public string AccessibleName
    {
        get => (string)GetValue(AccessibleNameProperty);
        set => SetValue(AccessibleNameProperty, value);
    }

    public string IncreaseAccessibleName
    {
        get => (string)GetValue(IncreaseAccessibleNameProperty);
        set => SetValue(IncreaseAccessibleNameProperty, value);
    }

    public string DecreaseAccessibleName
    {
        get => (string)GetValue(DecreaseAccessibleNameProperty);
        set => SetValue(DecreaseAccessibleNameProperty, value);
    }

    private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var control = (NumericStepControl)dependencyObject;
        var clamped = control.Clamp((int)args.NewValue);
        if (clamped != (int)args.NewValue)
        {
            control.SetCurrentValue(ValueProperty, clamped);
        }

        // Update automation name to include value for screen readers
        control.UpdateAutomationName();
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

    private static void OnAccessibleNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var control = (NumericStepControl)dependencyObject;
        control.UpdateAutomationName();
    }

    private void UpdateAutomationName()
    {
        // Combine accessible name with current value for screen readers
        var fullName = string.IsNullOrEmpty(AccessibleName)
            ? Value.ToString()
            : $"{AccessibleName} {Value}";
        System.Windows.Automation.AutomationProperties.SetName(this, fullName);
    }

    private void Root_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        bool handled = false;

        switch (e.Key)
        {
            case System.Windows.Input.Key.Up:
            case System.Windows.Input.Key.Right:
                SetCurrentValue(ValueProperty, Clamp(Value + Step));
                BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
                handled = true;
                break;

            case System.Windows.Input.Key.Down:
            case System.Windows.Input.Key.Left:
                SetCurrentValue(ValueProperty, Clamp(Value - Step));
                BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
                handled = true;
                break;

            case System.Windows.Input.Key.PageUp:
                SetCurrentValue(ValueProperty, Clamp(Value + (Step * 10)));
                BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
                handled = true;
                break;

            case System.Windows.Input.Key.PageDown:
                SetCurrentValue(ValueProperty, Clamp(Value - (Step * 10)));
                BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
                handled = true;
                break;

            case System.Windows.Input.Key.Home:
                SetCurrentValue(ValueProperty, Minimum);
                BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
                handled = true;
                break;

            case System.Windows.Input.Key.End:
                SetCurrentValue(ValueProperty, Maximum);
                BindingOperations.GetBindingExpression(this, ValueProperty)?.UpdateSource();
                handled = true;
                break;
        }

        if (handled)
        {
            e.Handled = true;
        }
    }

    private int Clamp(int value)
    {
        var minimum = Minimum;
        var maximum = Math.Max(minimum, Maximum);
        return Math.Min(Math.Max(value, minimum), maximum);
    }
}

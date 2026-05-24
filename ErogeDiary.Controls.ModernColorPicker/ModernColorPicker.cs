using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ErogeDiary.Controls.ModernColorPicker;

[TemplatePart(Name = ColorFieldPartName, Type = typeof(FrameworkElement))]
[TemplatePart(Name = BrightnessSliderPartName, Type = typeof(FrameworkElement))]
[TemplatePart(Name = ColorFieldIndicatorPartName, Type = typeof(Ellipse))]
[TemplatePart(Name = BrightnessSliderIndicatorPartName, Type = typeof(FrameworkElement))]
public class ModernColorPicker : Control
{
    private const string ColorFieldPartName = "PART_ColorField";
    private const string BrightnessSliderPartName = "PART_BrightnessSlider";
    private const string ColorFieldIndicatorPartName = "PART_ColorFieldIndicator";
    private const string BrightnessSliderIndicatorPartName = "PART_BrightnessSliderIndicator";

    private static readonly Color DefaultAccentColor = Color.FromRgb(0x5B, 0x2C, 0x6F);
    private static readonly Regex HexRgbPattern = new("^#[0-9a-fA-F]{6}$", RegexOptions.Compiled);

    private FrameworkElement? colorField;
    private FrameworkElement? brightnessSlider;
    private Ellipse? colorFieldIndicator;
    private FrameworkElement? brightnessSliderIndicator;
    private bool isSynchronizing;
    private ColorPickerState pickerState = ColorPickerState.FromColor(DefaultAccentColor, 0);

    static ModernColorPicker()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernColorPicker),
            new FrameworkPropertyMetadata(typeof(ModernColorPicker)));
    }

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ModernColorPicker),
            new FrameworkPropertyMetadata(
                DefaultAccentColor,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedColorChanged));

    public string ColorCode
    {
        get => (string)GetValue(ColorCodeProperty);
        set => SetValue(ColorCodeProperty, value);
    }

    public static readonly DependencyProperty ColorCodeProperty =
        DependencyProperty.Register(
            nameof(ColorCode),
            typeof(string),
            typeof(ModernColorPicker),
            new FrameworkPropertyMetadata(
                ToColorCode(DefaultAccentColor),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnColorCodeChanged));

    public Color FieldBaseColor
    {
        get => (Color)GetValue(FieldBaseColorProperty);
        private set => SetValue(FieldBaseColorPropertyKey, value);
    }

    private static readonly DependencyPropertyKey FieldBaseColorPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(FieldBaseColor),
            typeof(Color),
            typeof(ModernColorPicker),
            new PropertyMetadata(DefaultAccentColor));

    public static readonly DependencyProperty FieldBaseColorProperty =
        FieldBaseColorPropertyKey.DependencyProperty;

    public override void OnApplyTemplate()
    {
        DetachHandlers();

        base.OnApplyTemplate();

        colorField = GetTemplateChild(ColorFieldPartName) as FrameworkElement;
        brightnessSlider = GetTemplateChild(BrightnessSliderPartName) as FrameworkElement;
        colorFieldIndicator = GetTemplateChild(ColorFieldIndicatorPartName) as Ellipse;
        brightnessSliderIndicator = GetTemplateChild(BrightnessSliderIndicatorPartName) as FrameworkElement;

        AttachHandlers();
        SyncPickerStateFromSelectedColor();
        UpdateVisualState();
    }

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ModernColorPicker)d;
        if (control.isSynchronizing)
        {
            return;
        }

        var color = ForceOpaque((Color)e.NewValue);
        if (color != (Color)e.NewValue)
        {
            control.Synchronize(() =>
            {
                control.SetCurrentValue(SelectedColorProperty, color);
                control.SetCurrentValue(ColorCodeProperty, ToColorCode(color));
            });
            control.SyncPickerStateFromSelectedColor();
            control.UpdateVisualState();
            return;
        }

        control.Synchronize(() => control.SetCurrentValue(ColorCodeProperty, ToColorCode(color)));
        control.SyncPickerStateFromSelectedColor();
        control.UpdateVisualState();
    }

    private static void OnColorCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ModernColorPicker)d;
        if (control.isSynchronizing)
        {
            return;
        }

        var newValue = e.NewValue as string;
        if (!TryParseColorCode(newValue, out var color))
        {
            control.Synchronize(() => control.SetCurrentValue(ColorCodeProperty, e.OldValue));
            return;
        }

        control.Synchronize(() =>
        {
            control.SetCurrentValue(SelectedColorProperty, color);
            control.SetCurrentValue(ColorCodeProperty, ToColorCode(color));
        });
        control.SyncPickerStateFromSelectedColor();
        control.UpdateVisualState();
    }

    private void Synchronize(Action action)
    {
        isSynchronizing = true;
        try
        {
            action();
        }
        finally
        {
            isSynchronizing = false;
        }
    }

    private static bool TryParseColorCode(string? value, out Color color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(value) || !HexRgbPattern.IsMatch(value))
        {
            return false;
        }

        color = Color.FromRgb(
            byte.Parse(value.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(value.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(value.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        return true;
    }

    private static Color ForceOpaque(Color color) =>
        Color.FromRgb(color.R, color.G, color.B);

    private static string ToColorCode(Color color) =>
        FormattableString.Invariant($"#{color.R:X2}{color.G:X2}{color.B:X2}");

    private void AttachHandlers()
    {
        if (colorField != null)
        {
            colorField.MouseLeftButtonDown += ColorFieldMouseLeftButtonDown;
            colorField.MouseMove += ColorFieldMouseMove;
            colorField.MouseLeftButtonUp += ColorFieldMouseLeftButtonUp;
            colorField.SizeChanged += ColorFieldSizeChanged;
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.MouseLeftButtonDown += BrightnessSliderMouseLeftButtonDown;
            brightnessSlider.MouseMove += BrightnessSliderMouseMove;
            brightnessSlider.MouseLeftButtonUp += BrightnessSliderMouseLeftButtonUp;
            brightnessSlider.SizeChanged += BrightnessSliderSizeChanged;
        }
    }

    private void DetachHandlers()
    {
        if (colorField != null)
        {
            colorField.MouseLeftButtonDown -= ColorFieldMouseLeftButtonDown;
            colorField.MouseMove -= ColorFieldMouseMove;
            colorField.MouseLeftButtonUp -= ColorFieldMouseLeftButtonUp;
            colorField.SizeChanged -= ColorFieldSizeChanged;
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.MouseLeftButtonDown -= BrightnessSliderMouseLeftButtonDown;
            brightnessSlider.MouseMove -= BrightnessSliderMouseMove;
            brightnessSlider.MouseLeftButtonUp -= BrightnessSliderMouseLeftButtonUp;
            brightnessSlider.SizeChanged -= BrightnessSliderSizeChanged;
        }
    }

    private void ColorFieldMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        colorField?.CaptureMouse();
        UpdateFromColorFieldPoint(e.GetPosition(colorField));
    }

    private void ColorFieldMouseMove(object sender, MouseEventArgs e)
    {
        if (colorField?.IsMouseCaptured == true)
        {
            UpdateFromColorFieldPoint(e.GetPosition(colorField));
        }
    }

    private void ColorFieldMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        colorField?.ReleaseMouseCapture();
    }

    private void BrightnessSliderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        brightnessSlider?.CaptureMouse();
        UpdateFromBrightnessPoint(e.GetPosition(brightnessSlider));
    }

    private void BrightnessSliderMouseMove(object sender, MouseEventArgs e)
    {
        if (brightnessSlider?.IsMouseCaptured == true)
        {
            UpdateFromBrightnessPoint(e.GetPosition(brightnessSlider));
        }
    }

    private void BrightnessSliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        brightnessSlider?.ReleaseMouseCapture();
    }

    private void ColorFieldSizeChanged(object sender, SizeChangedEventArgs e) => UpdateIndicatorPositions();

    private void BrightnessSliderSizeChanged(object sender, SizeChangedEventArgs e) => UpdateIndicatorPositions();

    private void UpdateFromColorFieldPoint(Point point)
    {
        if (colorField == null || colorField.ActualWidth <= 0 || colorField.ActualHeight <= 0)
        {
            return;
        }

        pickerState = pickerState with
        {
            Hue = ColorPickerColorConverter.Clamp(point.X / colorField.ActualWidth) * 360,
            Tint = ColorPickerColorConverter.Clamp(point.Y / colorField.ActualHeight)
        };
        ApplyPickerState();
    }

    private void UpdateFromBrightnessPoint(Point point)
    {
        if (brightnessSlider == null || brightnessSlider.ActualWidth <= 0)
        {
            return;
        }

        pickerState = pickerState with
        {
            Brightness = ColorPickerColorConverter.Clamp(point.X / brightnessSlider.ActualWidth)
        };
        ApplyPickerState();
    }

    private void ApplyPickerState()
    {
        var color = pickerState.ToColor();
        Synchronize(() =>
        {
            SetCurrentValue(SelectedColorProperty, color);
            SetCurrentValue(ColorCodeProperty, ToColorCode(color));
        });
        UpdateVisualState();
    }

    private void SyncPickerStateFromSelectedColor() =>
        pickerState = ColorPickerState.FromColor(SelectedColor, pickerState.Hue);

    private void UpdateVisualState()
    {
        SetValue(FieldBaseColorPropertyKey,
            ColorPickerColorConverter.Mix(
                ColorPickerColorConverter.ColorFromHsv(pickerState.Hue, 1, 1),
                Colors.White,
                pickerState.Tint));
        UpdateIndicatorPositions();
    }

    private void UpdateIndicatorPositions()
    {
        if (colorField != null && colorFieldIndicator != null)
        {
            Canvas.SetLeft(colorFieldIndicator, pickerState.Hue / 360 * colorField.ActualWidth - colorFieldIndicator.Width / 2);
            Canvas.SetTop(colorFieldIndicator, pickerState.Tint * colorField.ActualHeight - colorFieldIndicator.Height / 2);
        }

        if (brightnessSlider != null && brightnessSliderIndicator != null)
        {
            Canvas.SetLeft(brightnessSliderIndicator, pickerState.Brightness * brightnessSlider.ActualWidth - brightnessSliderIndicator.Width / 2);
        }
    }
}

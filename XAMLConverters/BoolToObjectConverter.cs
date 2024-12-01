using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScreTran;

/// <summary>
/// This class converts a boolean value into an other object.
/// Can be used to convert true/false to visibility, a couple of colors, couple of images, etc.
/// </summary>
public partial class BoolToObjectConverter : DependencyObject, IValueConverter
{
    /// <summary>
    /// Identifies the <see cref="TrueValue"/> property.
    /// </summary>
    public static readonly DependencyProperty TrueValueProperty =
        DependencyProperty.Register(nameof(TrueValue), typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="FalseValue"/> property.
    /// </summary>
    public static readonly DependencyProperty FalseValueProperty =
        DependencyProperty.Register(nameof(FalseValue), typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the value to be returned when the boolean is true
    /// </summary>
    public object TrueValue
    {
        get => GetValue(TrueValueProperty);
        set => SetValue(TrueValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to be returned when the boolean is false
    /// </summary>
    public object FalseValue
    {
        get => GetValue(FalseValueProperty);
        set => SetValue(FalseValueProperty, value);
    }

    /// <summary>
    /// Convert a boolean value to an other object.
    /// </summary>
    /// <param name="value">The source data being passed to the target.</param>
    /// <param name="targetType">The type of the target property, as a type reference.</param>
    /// <param name="parameter">An optional parameter to be used to invert the converter logic.</param>
    /// <param name="culture">The language of the conversion.</param>
    /// <returns>The value to be passed to the target dependency property.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool && (bool)value ? TrueValue : FalseValue;
    }

    /// <summary>
    /// Convert back the value to a boolean
    /// </summary>
    /// <remarks>If the <paramref name="value"/> parameter is a reference type, <see cref="TrueValue"/> must match its reference to return true.</remarks>
    /// <param name="value">The target data being passed to the source.</param>
    /// <param name="targetType">The type of the target property, as a type reference (System.Type for Microsoft .NET, a TypeName helper struct for Visual C++ component extensions (C++/CX)).</param>
    /// <param name="parameter">An optional parameter to be used to invert the converter logic.</param>
    /// <param name="culture">The language of the conversion.</param>
    /// <returns>The value to be passed to the source object.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Equals(value, TrueValue);
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace BancaJornal.Desktop.Converters;

public class BoolToTextConverter : IValueConverter
{
    public string TrueText { get; set; } = "Edição";
    public string FalseText { get; set; } = "Novo Produto";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? TrueText : FalseText;
        return FalseText;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace AddressCoding
{
    /// <summary>
    /// Класс конвертер процентов выполненной работы в значение от 0 до 1
    /// </summary>
    public class ConverterStatToProgress : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? ((double)value) / 100.0 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
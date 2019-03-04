using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace AddressCoding
{
    /// <summary>
    /// Класс конвертер запущенного процесса в состояние индикатора выполнения на панели задач
    /// </summary>
    public class ConverterStatOrponingToProgressState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) && (bool)value ? TaskbarItemProgressState.Normal : TaskbarItemProgressState.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
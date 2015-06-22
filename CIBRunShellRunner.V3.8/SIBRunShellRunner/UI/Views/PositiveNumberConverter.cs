using System;
using System.Windows.Data;

namespace CIBRunShellRunner.Views
{
    class PositiveNumberConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int result;
            string str = value.ToString();
            if (Int32.TryParse(str, out result))
            {
                if (result <= 1)
                    result = 1;
            }
            else
            {
                result = 1;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int result;
            string str = value.ToString();
            if (Int32.TryParse(str, out result))
            {
                if (result <= 1)
                    result = 1;

            }
            else
            {
                result = -1;
            }
            return result;
        }
    }
}

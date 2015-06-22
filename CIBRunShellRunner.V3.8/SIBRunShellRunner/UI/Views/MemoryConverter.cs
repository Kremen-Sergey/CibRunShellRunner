using System;
using System.Windows.Data;

namespace CIBRunShellRunner.Views
{
    class MemoryConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int64 valueInt = (Int64)value;
            if (valueInt < 1024)////[...,1kB)
                return (valueInt + "B");
            if ((valueInt>=1024)&&(valueInt<1024*1024))//[1kb,1MB)
                return (valueInt/1024 + "kB");
            if ((valueInt >= 1024 * 1024) && (valueInt < 1024 * 1024 * 1024))//[1Mb,1GB)
                return (valueInt/1024/1024 + "MB");
            return (valueInt / 1024 / 1024 /1024+ "GB");//[1Gb,...)
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

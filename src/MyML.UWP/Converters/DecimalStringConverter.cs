using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters
{
    public class DecimalStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return string.Empty;

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return value;

            return decimal.Parse(value as string);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters
{
    public sealed class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return String.Empty;

            if (parameter == null)
                return value;

            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            
            throw new NotImplementedException();
        }
    }
}

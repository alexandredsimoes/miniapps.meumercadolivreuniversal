using System;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            //
            return string.Format("file://{0}", value);
            return null;
 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

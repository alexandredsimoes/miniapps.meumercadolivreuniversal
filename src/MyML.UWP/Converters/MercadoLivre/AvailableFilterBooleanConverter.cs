using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class AvailableFilterBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return false;
            return value.ToString().ToLowerInvariant().Equals("yes") ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;
            return (bool)value ? "yes" : "no";
        }
    }
}

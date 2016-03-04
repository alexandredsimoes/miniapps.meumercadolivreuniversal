using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters
{
    public class ValueToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return false;
            if (parameter == null)
                throw new Exception("Os parametros 'parameter', não devem ser nulos");

            return value.ToString().Equals(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return parameter;
        }
    }
}

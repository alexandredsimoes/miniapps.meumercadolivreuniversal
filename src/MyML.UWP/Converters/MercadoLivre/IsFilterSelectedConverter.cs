using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class IsFilterSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

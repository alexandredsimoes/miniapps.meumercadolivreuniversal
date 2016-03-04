using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class FavoritesOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 0.50D;

            if ((bool)value == true)
                return 1D;

            return 0.50D;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class ReputationHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 5;

            if (value.ToString().Equals(parameter.ToString()))
                return 12;

            return 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

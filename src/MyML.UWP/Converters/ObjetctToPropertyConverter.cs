using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using System.Reflection;
using MyML.UWP.Models.Mercadolivre;
using System.Linq;

namespace MyML.UWP.Converters
{
    public class ObjetctToPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var prop = value.GetType().GetRuntimeProperty(parameter.ToString());

            if (prop != null)
                return prop.GetValue(value);
            //var props = value.GetType().GetRuntimeProperties().ToList();
            //var product = value.GetType().GetRuntimeProperty("Key").GetValue(value);

            //if (product != null)
            //    return product.GetType().GetRuntimeProperty(parameter.ToString()).GetValue(product);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

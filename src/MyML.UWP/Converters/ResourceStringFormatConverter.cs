using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters
{
    public class ResourceStringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resourceLoader = new ResourceLoader();
            if (value == null) return value;
            if (parameter == null)
                throw new ArgumentNullException("O parametro 'parameter', precisa ser informado com o nome da resource string.");

            var resourceString = resourceLoader.GetString(parameter.ToString());
            return string.Format(resourceString, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

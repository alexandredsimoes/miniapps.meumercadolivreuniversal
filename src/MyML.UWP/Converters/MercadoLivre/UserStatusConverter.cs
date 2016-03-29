using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class UserStatusConverter : IValueConverter
    {
        private readonly ResourceLoader _resourceLoader;

        public UserStatusConverter()
        {
            _resourceLoader = new ResourceLoader();// ServiceLocator.Current.GetInstance<ResourceLoader>();
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return _resourceLoader.GetString("UserStatusNull");

            return _resourceLoader.GetString($"UserStatus{value.ToString().ToLowerInvariant()}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

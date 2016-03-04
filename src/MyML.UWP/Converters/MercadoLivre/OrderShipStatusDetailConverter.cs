using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class OrderShipStatusDetailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;
            var resource = new ResourceLoader();
            switch (value.ToString())
            {
                case "delivered":
                    return resource.GetString("MLOrderShipStatusDelivered");
                case "pending":
                    return resource.GetString("MLOrderShipStatusPending");
                case "handling":
                case "ready_to_ship":
                    return resource.GetString("MLOrderShipStatusHandling");
                case "shipped":
                    return resource.GetString("MLOrderShipStatusShipped");
                case "not_delivered":
                    return resource.GetString("MLOrderShipStatusNotDelivered");
                case "cancelled":
                    return resource.GetString("MLOrderShipStatusCancelled");
                case "not_verified":
                    return resource.GetString("MLOrderShipStatusNotVerified");
                    
                default:
                    return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

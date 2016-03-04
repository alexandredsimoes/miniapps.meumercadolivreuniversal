using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class OrderStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;
            var resource = new ResourceLoader();          
            switch (value.ToString())
            {
                case "paid":
                    return resource.GetString("MLStatusPaid");
                case "confirmed":
                    return resource.GetString("MLStatusConfirmed");
                case "payment_required":
                    return resource.GetString("MLStatusPaymentRequired");
                case "payment_in_process":
                    return resource.GetString("MLStatusPaymentProccess");
                case "cancelled":
                    return resource.GetString("MLStatusPaymentCancelled");

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

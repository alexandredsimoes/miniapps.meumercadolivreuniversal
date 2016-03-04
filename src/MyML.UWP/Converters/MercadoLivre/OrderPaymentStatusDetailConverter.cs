using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class OrderPaymentStatusDetailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;
            var resource = new ResourceLoader();
            switch (value.ToString())
            {
                case "accredited":
                    return resource.GetString("MLOrderPaymentStatusDetailAccredited");
                case "refunded":
                    return resource.GetString("MLOrderPaymentStatusDetailRefunded");

                case "pending":
                    return resource.GetString("MLOrderPaymentStatusDetailPending");
                case "approved":
                    return resource.GetString("MLOrderPaymentStatusDetailApproved");
                case "in_process":
                    return resource.GetString("MLOrderPaymentStatusDetailInProcess");
                case "rejected":
                    return resource.GetString("MLOrderPaymentStatusDetailRejected");
                case "cancelled":
                    return resource.GetString("MLOrderPaymentStatusDetailCancelled");
                case "in_mediation":
                    return resource.GetString("MLOrderPaymentStatusDetailInMediation");
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

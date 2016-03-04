using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class OrderPaymentTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;
            var resource = new ResourceLoader();
            switch (value.ToString())
            {
                case "credit_card":
                    return resource.GetString("MLOrderPaymentTypeCreditCard");
                case "account_money":
                    return resource.GetString("MLOrderPaymentTypeMoney");
                case "ticket":
                    return resource.GetString("MLOrderPaymentTypeTicket");                   
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

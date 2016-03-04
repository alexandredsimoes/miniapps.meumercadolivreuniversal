using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class FeedbackRatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;
            var resource = new ResourceLoader();
            switch (value.ToString())
            {
                case "positive":
                    return resource.GetString("MLFeedbackRatingPositive");
                case "negative":
                    return resource.GetString("MLFeedbackRatingNegative");
                case "neutral":
                    return resource.GetString("MLFeedbackRatingNeutral");
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

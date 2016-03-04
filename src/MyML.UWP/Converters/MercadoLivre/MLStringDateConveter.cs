using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters.MercadoLivre
{
    public class MLStringDateConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            if (value == null) return string.Empty;
            var data = DateTime.MinValue;

            
            if(DateTime.TryParse(value.ToString(), out data))
            {
                TimeSpan s = data.Subtract(DateTime.Now);
                return $" {s.Days} dias";
                return GetPrettyDate(data);
                //return data.Humanize(true).ToString();
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public string GetPrettyDate(DateTime d)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = d.Subtract(DateTime.Now);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0 || dayDiff >= 31)
            {
                return null;
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} minutes ago",
                        Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} hours ago",
                        Math.Floor((double)secDiff / 3600));
                }
            }
            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} days ago",
                dayDiff);
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} weeks ago",
                Math.Ceiling((double)dayDiff / 7));
            }
            return null;
        }
    }
}

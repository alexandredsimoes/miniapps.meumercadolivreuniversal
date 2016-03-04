using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters
{
    public class HumanizeDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;
            var data = DateTimeOffset.MinValue;
            if (DateTimeOffset.TryParse(value.ToString(), out data))
            {


                var ts = new TimeSpan(Math.Abs(DateTimeOffset.Now.Ticks - data.Ticks));


                if (ts.TotalMilliseconds < 500)
                    return "Muito pouco tempo";

                if (ts.TotalSeconds < 60)
                    return "Segundos";

                if (ts.TotalSeconds < 120)
                    return "1 minuto";

                if (ts.TotalMinutes < 60)
                    return $"{ts.Minutes} minutos";

                if (ts.TotalMinutes < 90)
                    return $"{1} hora";

                if (ts.TotalHours < 24)
                    return $"{ts.Hours} horas";

                if (ts.TotalHours < 48)
                {
                    var days = Math.Abs((data.Date - DateTimeOffset.Now.Date).Days);
                    return $"{days} dias";                    
                }

                if (ts.TotalDays < 28)                    
                    return ts.TotalDays == 1 ? $"{ts.Days} dia" : $"{ts.Days} dias";

                

                if (ts.TotalDays < 345)
                {
                    var months = System.Convert.ToInt32(Math.Floor(ts.TotalDays / 29.5));
                    return months > 1 ? $"{months} meses" : $"{months} mês";
                }

                var years = System.Convert.ToInt32(Math.Floor(ts.TotalDays / 365));
                if (years == 0) years = 1;

                return years == 1 ? $"{years} ano" : $"{years} ano(s)";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

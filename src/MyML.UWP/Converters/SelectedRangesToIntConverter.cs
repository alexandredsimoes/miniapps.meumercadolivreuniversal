using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Converters
{
    public class SelectedRangesToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var obj = value as IReadOnlyCollection<ItemIndexRange>;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

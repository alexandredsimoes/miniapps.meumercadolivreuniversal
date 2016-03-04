using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Aggregattor
{
    public class MessengerFilterDetails
    {
        public IList<AvailableFilter> SelectedFilters { get; set; }
        public IList<AvailableFilter> Filters { get; set; }
        public IList<AvailableSort> Sorts { get; set; }
    }
}

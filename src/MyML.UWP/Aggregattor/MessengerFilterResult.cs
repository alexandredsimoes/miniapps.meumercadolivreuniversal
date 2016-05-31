using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Aggregattor
{
    public class MessengerFilterResult
    {
        public string Query { get; set; }
        public string Result { get; set; }
        public IList<AvailableFilter> SelectedFilters { get; set; }
        public IList<AvailableSort> SelectedSorts { get; set; }
    }
}

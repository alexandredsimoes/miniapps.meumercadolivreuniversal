using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Adapters
{
    public interface IPagedResponse<T>
    {
        IEnumerable<T> Items { get; }
        int VirtualCount { get; }
        IList<AvailableFilter> Filters {get;}
        IList<AvailableSort> Sorts { get; }
    }
}

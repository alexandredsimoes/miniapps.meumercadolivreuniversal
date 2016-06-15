using System.Collections.Generic;
using MyML.UWP.Models.Mercadolivre;

namespace MyML.UWP.Adapters.Search
{
    public class OrdersResponse : IPagedResponse<MLOrderInfo>
    {
        public OrdersResponse(IEnumerable<MLOrderInfo> items, int virtualCount)
        {
            this.Items = items;
            this.VirtualCount = virtualCount;
        }

        public int VirtualCount { get; private set; }
        public IEnumerable<MLOrderInfo> Items { get; private set; }
        public IList<AvailableFilter> Filters { get; set; }
        public IList<string> SortStrings { get; set; }
        public IList<AvailableSort> Sorts { get; set; }

        public Paging Paging
        {
            get;
        }
    }
}
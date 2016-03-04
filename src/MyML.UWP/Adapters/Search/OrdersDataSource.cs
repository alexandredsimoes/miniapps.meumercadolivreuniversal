using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Adapters.Search
{
    public class OrdersDataSource : IPagedSource<MLOrderInfo>
    {
        private IMercadoLivreService _mercadoLivreServices;

        public OrdersDataSource()
        {
            _mercadoLivreServices = SimpleIoc.Default.GetInstance<IMercadoLivreService>();
        }

        public async Task<IPagedResponse<MLOrderInfo>> GetPage(string query, int pageIndex, int pageSize, bool recents)
        {
            try
            {
                var items = new MLOrder();
                if (recents)
                    items = await _mercadoLivreServices.ListRecentOrders(pageIndex, pageSize);
                else
                    items = await _mercadoLivreServices.ListOrders(pageIndex, pageSize);

                bool success = items == null ? false : items.paging.total > 0;

                if (success)
                {
                    int virtualCount;
                    virtualCount = items.paging.total;
                    return new OrdersResponse(items.results.AsEnumerable(), virtualCount);
                }

                return null;
                // throw new WebException("Sem conexão de internet");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }

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
    }
}

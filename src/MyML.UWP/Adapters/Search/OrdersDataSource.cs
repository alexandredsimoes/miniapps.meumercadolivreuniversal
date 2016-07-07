using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
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

        public async Task<IPagedResponse<MLOrderInfo>> GetPage(string query, int pageIndex, int pageSize, string status)
        {
            return await GetPage(query, pageIndex, pageSize, true);
        }

        public async Task<IPagedResponse<MLOrderInfo>> GetPage(string query, int pageIndex, int pageSize, bool recents)
        {
            try
            {
                MLOrder items;
                if (recents)
                    items = await _mercadoLivreServices.ListRecentOrders(pageIndex, pageSize);
                else
                    items = await _mercadoLivreServices.ListOrders(pageIndex, pageSize);

                bool success = items != null && items.paging.total > 0;

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

        public async Task<IPagedResponse<MLOrderInfo>> GetPage(string query, int pageIndex, int pageSize, bool searchByName, bool? highResolutionImages)
        {
            return await GetPage(query, pageIndex, pageSize, true);
        }
    }
}

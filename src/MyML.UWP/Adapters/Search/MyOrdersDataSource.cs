using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;

namespace MyML.UWP.Adapters.Search
{
    public class MyOrdersDataSource : IPagedSource<MLOrderInfo>
    {
        private IMercadoLivreService _mercadoLivreServices;

        public MyOrdersDataSource()
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
                MLOrder items = await _mercadoLivreServices.ListMyOrders(pageIndex, pageSize, new KeyValuePair<string, object>[] { });

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
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
    public class OrdersArchivedDataSource : IPagedSource<MLOrderInfo>
    {
        private readonly IMercadoLivreService _mercadoLivreServices;

        public OrdersArchivedDataSource()
        {
            _mercadoLivreServices = SimpleIoc.Default.GetInstance<IMercadoLivreService>();
        }


        public async Task<IPagedResponse<MLOrderInfo>> GetPage(string query, int pageIndex, int pageSize, bool recents)
        {
            try
            {
                MLOrder items;
                items = await _mercadoLivreServices.ListArchivedSellerOrders(pageIndex, pageSize);
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

        public Task<IPagedResponse<MLOrderInfo>> GetPage(string query, int pageIndex, int pageSize, bool searchByName, bool? highResolutionImages)
        {
            throw new NotImplementedException();
        }
    }
}

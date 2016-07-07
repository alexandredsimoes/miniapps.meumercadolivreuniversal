using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Adapters.Search
{
    public class AdvertsDataSource : IPagedSource<Item>
    {
        private IMercadoLivreService _mercadoLivreService;

        public AdvertsDataSource()
        {
            _mercadoLivreService = SimpleIoc.Default.GetInstance<IMercadoLivreService>();
        }

        public Task<IPagedResponse<Item>> GetPage(string query, int pageIndex, int pageSize, string searchByName)
        {
            throw new NotImplementedException();
        }

        public async Task<IPagedResponse<Item>> GetPage(string query, int pageIndex, int pageSize, bool searchByName)
        {
            try
            {
                var items = await _mercadoLivreService.ListMyItems(pageIndex, pageSize,
                                    new KeyValuePair<string, object>[]
                                    {
                                        new KeyValuePair<string, object>("status",query)
                                    });

                items.ListTypes = await _mercadoLivreService.ListTypes(Consts.ML_ID_BRASIL);
                if (items.results_graph == null)
                    items.results_graph = new List<Item>();

                foreach (var item in items.results)
                {
                    var product = await _mercadoLivreService
                        .GetItemDetails(item,
                            new KeyValuePair<string, object>[]
                            {
                                new KeyValuePair<string, object>("attributes",
                                    "id,title,price,thumbnail,stop_time,available_quantity")
                            })
                        .ConfigureAwait(false);

                    if (product != null)
                    {
                        if (items.ListTypes != null)
                            product.ListType = items.ListTypes.FirstOrDefault(c => c.id == product.listing_type_id);

                        items.results_graph.Add(product);
                    }
                }

                bool success = items == null ? false : items.paging.total > 0;

                if (success)
                {
                    int virtualCount;
                    virtualCount = items.paging.total;
                    return new AdvertsResponse(items.results_graph.AsEnumerable(), virtualCount);
                }

                return null;
                // throw new WebException("Sem conexão de internet");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IPagedResponse<Item>> GetPage(string query, int pageIndex, int pageSize, bool searchByName, bool? highResolutionImages)
        {
            return await GetPage(query, pageIndex, pageSize, searchByName);
        }
    }
    public class AdvertsResponse : IPagedResponse<Item>
    {
        public AdvertsResponse(IEnumerable<Item> items, int virtualCount)
        {
            this.Items = items;
            this.VirtualCount = virtualCount;
        }

        public int VirtualCount { get; private set; }
        public IEnumerable<Item> Items { get; private set; }
        public IList<AvailableFilter> Filters { get; set; }
        public IList<string> SortStrings { get; set; }
        public IList<AvailableSort> Sorts { get; set; }

        public Paging Paging
        {
            get;
        }
    }
}

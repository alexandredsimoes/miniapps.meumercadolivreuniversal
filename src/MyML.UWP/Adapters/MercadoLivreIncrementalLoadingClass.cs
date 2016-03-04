using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyML.UWP.Adapters
{
    public class MercadoLivreIncrementalLoadingClass<T> : IncrementalLoadingBase
    {
        private readonly IMercadoLivreService _mercadoLivreServices;
        private string Query { get; set; }
        private bool SearchByName { get; set; }
        private int VirtualCount { get; set; }
        private int CurrentPage { get; set; }
        private int PageSize { get; set; }

        public MercadoLivreIncrementalLoadingClass(int pageIndex, int pageSize, string query = null, bool searchByProductName = false)
        {
            _mercadoLivreServices = SimpleIoc.Default.GetInstance<IMercadoLivreService>();
            this.CurrentPage = pageIndex;
            this.PageSize = pageSize;
            this.Query = query;
            this.VirtualCount = int.MaxValue;
            this.SearchByName = searchByProductName;
        }

        protected override bool HasMoreItemsOverride()
        {
            return this.VirtualCount > this.CurrentPage * this.PageSize;
        }

        protected async override Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            var items = new MLSearchResult();

            if (!SearchByName)
                items = await _mercadoLivreServices.ListProductsByCategory(Query, CurrentPage++, PageSize);
            else
                items = await _mercadoLivreServices.ListProductsByName(Query, CurrentPage++, PageSize);


            bool success = items == null ? false : items.results.Count > 0;
            VirtualCount = items == null ? 0 : items.paging.total;
            return items.results.ToArray();
        }
    }
}

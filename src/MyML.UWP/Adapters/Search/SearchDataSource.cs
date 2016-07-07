using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Diagnostics;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Web.Syndication;
using System.Text.RegularExpressions;
using MyML.UWP.Services;

using Windows.ApplicationModel.Resources;
using System.Net;
using MyML.UWP.Models.Mercadolivre;
using GalaSoft.MvvmLight.Ioc;

namespace MyML.UWP.Adapters.Search
{
    public class SearchDataSource : IPagedSource<Item>
    {
        private readonly IMercadoLivreService _mercadoLivreServices;

        public SearchDataSource()
        {
            _mercadoLivreServices = SimpleIoc.Default.GetInstance<IMercadoLivreService>();            
        }

        public Task<IPagedResponse<Item>> GetPage(string query, int pageIndex, int pageSize, bool searchByName)
        {
            throw new NotImplementedException();
        }

        public async Task<IPagedResponse<Item>> GetPage(string query, int pageIndex, int pageSize, bool searchByName, bool? highResolutionImages)
        {                        
            try
            {
                MLSearchResult items;

                if (!searchByName)
                    items = await _mercadoLivreServices.ListProductsByCategory(query, pageIndex, pageSize);
                else
                    items = await _mercadoLivreServices.ListProductsByName(query, pageIndex, pageSize);                


                bool success = items != null && items.results.Count > 0;

                if (success)
                {
                    if (highResolutionImages ?? false)
                    {
                        //Tenta obter as imagens dos produtos
                        foreach (var result in items.results)
                        {
                            var pictures =
                                await _mercadoLivreServices.GetItemDetails(result.id, new KeyValuePair<string, object>[]
                                {
                                    new KeyValuePair<string, object>("attributes", "pictures"),
                                }).ConfigureAwait(false);
                            result.thumbnail = pictures.pictures[0].url;
                        }
                    }
                    var sorts = items.available_sorts;
                    var filters = items.available_filters;
                    int virtualCount;
                    virtualCount = items.paging.total;
                    return new ItensResponse(items.results.AsEnumerable(), virtualCount, filters, sorts, items.paging);
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


    [DebuggerDisplay("PageIndex = {PageIndex} - PageSize = {PageSize} - VirtualCount = {VirtualCount}")]
    public class ItensResponse : IPagedResponse<Item>
    {
        public ItensResponse(IEnumerable<Item> items, int virtualCount, IList<AvailableFilter> filters, IList<AvailableSort> sorts, Paging pagingInfo)
        {
            this.Items = items;
            this.VirtualCount = virtualCount;
            this.Filters = filters;
            this.Sorts = sorts;
            this.Paging = pagingInfo;
        }
        
        public int VirtualCount { get; private set; }
        public IEnumerable<Item> Items { get; private set; }
        public IList<AvailableFilter> Filters { get; set; }
        public IList<AvailableSort> Sorts { get; set; }
        public Paging Paging { get;  }
    }
}

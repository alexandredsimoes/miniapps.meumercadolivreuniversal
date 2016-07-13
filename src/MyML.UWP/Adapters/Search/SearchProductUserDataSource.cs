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
    public class SearchProductUserDataSource : IPagedSource<Item>
    {
        private readonly IMercadoLivreService _mercadoLivreServices;

        public SearchProductUserDataSource()
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
                MLSearchResult items = await _mercadoLivreServices.ListProductsByUser(query, pageIndex, pageSize);


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
}

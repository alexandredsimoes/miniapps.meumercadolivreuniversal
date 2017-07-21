using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.AppStorage;
using Newtonsoft.Json;

namespace MyML.UWP.Services
{
    public static class BaseServices<T>
    {
        private static readonly HttpClient Client = SimpleIoc.Default.GetInstance<HttpClient>();

        public static async Task<T> GetAsync(string url, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            T result = default(T);
            if (attributesAndFilters != null && attributesAndFilters.Length > 0)
            {
                for (int i = 0; i < attributesAndFilters.Length; i++)
                {
                    url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesAndFilters[i].Key, "=",
                        attributesAndFilters[i].Value, i < (attributesAndFilters.Length - 1) ? "," : string.Empty);
                }
            }

            await Client.GetAsync(url, HttpCompletionOption.ResponseContentRead)
                .ContinueWith(async c =>
                {
                    try
                    {
                        var r = c.Result;
                        if (r.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var s = await r.Content.ReadAsStringAsync().ConfigureAwait(false);
                            result = JsonConvert.DeserializeObject<T>(s);
                        }
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return default(T);                        
                    }
                    
                }).ConfigureAwait(false);
            return result;            
        }
    }
}

﻿using MyML.UWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MyML.UWP.Models.Mercadolivre;
using System.Diagnostics;
using System.Net.Http.Headers;
using MyML.UWP.AppStorage;
using Windows.Globalization.NumberFormatting;
using Windows.Storage;

namespace MyML.UWP.Services
{
    public class MercadoLivreServices : IMercadoLivreService
    {
        private readonly HttpClient _httpClient;
        private readonly IDataService _dataService;
        public MercadoLivreServices(HttpClient httpClient, IDataService dataService)
        {
            _httpClient = httpClient;
            _dataService = dataService;
        }

        public async Task<IList<MLCategorySearchResult>> ListCategories(string paisId)
        {
            try
            {
                var url = String.Format(Consts.ML_URL_CATEGORIAS, Consts.ML_ID_BRASIL);
                var r = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var s = await r.Content.ReadAsStringAsync();
                    var lista = JsonConvert.DeserializeObject<IList<MLCategorySearchResult>>(s);
                    return lista;
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("ListCategories", ex);
            }
            return null;
        }

        #region Métodos referente as questões
        public async Task<Models.Mercadolivre.MLQuestionResultSearch> ListQuestions(params KeyValuePair<string, object>[] attributesAndFilters)
        {
            try
            {
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);


                var url = Consts.GetUrl(Consts.ML_QUESTIONS_URL, userId, accessToken);

                if (attributesAndFilters != null && attributesAndFilters.Length > 0)
                {
                    for (int i = 0; i < attributesAndFilters.Length; i++)
                    {
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesAndFilters[i].Key, "=", attributesAndFilters[i].Value, i < (attributesAndFilters.Length - 1) ? "," : string.Empty);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLQuestionResultSearch>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MeuMercadoLivreServices.ListQuestions()", ex);
                return null;
            }
        }

        public async Task<bool> AnswerQuestion(string questionId, string content)
        {
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_QUESTIONS_ANSWER_URL, accessToken);

            //Monta os parametros
            var parametros = new
            {
                question_id = questionId,
                text = content
            };



            _httpClient.DefaultRequestHeaders.Accept.Clear();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



            var json = new StringContent(JsonConvert.SerializeObject(parametros));
            var response = await _httpClient.PostAsync(url, json);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveQuestion(string questionId)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_REMOVE_QUESTION_URL, questionId, accessToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            var response = await _httpClient.DeleteAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<ProductQuestion> ListQuestionsByProduct(string itemId, int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_QUESTIONS, itemId);

            if (pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            if (attributesAndFilters != null && attributesAndFilters.Length > 0)
            {
                for (int i = 0; i < attributesAndFilters.Length; i++)
                {
                    url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesAndFilters[i].Key, "=", attributesAndFilters[i].Value, i < (attributesAndFilters.Length - 1) ? "," : string.Empty);
                }
            }
            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<ProductQuestion>(await response.Content.ReadAsStringAsync());
                return result;
            }
            return null;
        }

        public async Task<ProductQuestionContent> GetQuestionDetails(string questionId, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            try
            {
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);


                var url = Consts.GetUrl(Consts.ML_QUESTIONS_DETAIL_URL, questionId);

                if (attributesAndFilters != null && attributesAndFilters.Length > 0)
                {
                    for (int i = 0; i < attributesAndFilters.Length; i++)
                    {
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesAndFilters[i].Key, "=", attributesAndFilters[i].Value, i < (attributesAndFilters.Length - 1) ? "," : string.Empty);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<ProductQuestionContent>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetQuestionDetails()", ex);
                return null;
            }
        }

        //public async Task<MLQuestionResultSearch> ListQuestions(KeyValuePair<string, object>[] attributesOrFilters)
        //{
        //    try
        //    {
        //        var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
        //        var url = String.Format(Consts.ML_QUESTIONS_LIST, accessToken);

        //        if (attributesOrFilters != null && attributesOrFilters.Length > 0)
        //        {
        //            for (int i = 0; i < attributesOrFilters.Length; i++)
        //            {
        //                url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesOrFilters[i].Key, "=", attributesOrFilters[i].Value, i < (attributesAndFilters.Length - 1) ? "," : string.Empty);
        //            }
        //        }

        //        var r = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        //        if (r.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var s = await r.Content.ReadAsStringAsync();
        //            var lista = JsonConvert.DeserializeObject<MLQuestionResultSearch>(s);
        //            return lista;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppLogs.WriteError("MercadoLivreServices.ListQuestions()", ex);
        //    }
        //    return null;
        //}
        #endregion Métodos referente as questões

        #region Métodos referente aos usuários
        public async Task<MLUserInfoSearchResult> GetUserInfo(string userId, params KeyValuePair<string, string>[] attributesOrFilters)
        {
            try
            {
                var url = Consts.GetUrl(Consts.ML_USER_INFO_URL, userId);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                url = string.Concat(url, "?", accessToken);

                if (attributesOrFilters != null && attributesOrFilters.Length > 0)
                {
                    for (int i = 0; i < attributesOrFilters.Length; i++)
                    {
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesOrFilters[i].Key, "=", attributesOrFilters[i].Value, i < (attributesOrFilters.Length - 1) ? "," : string.Empty);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLUserInfoSearchResult>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetUserInfo()", ex);
                return null;
            }
        }

        public async Task<MLUserInfoSearchResult> GetUserProfile()
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_USER_PROFILE, accessToken);
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLUserInfoSearchResult>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MeuMercadoLivreServices.GetUserProfile()", ex);
                return null;
            }
        }

        public async Task<IList<UserAddress>> GetUserAddress(int userId)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.ML_USER_ADDRESSES, userId, accessToken);
            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<IList<UserAddress>>(await response.Content.ReadAsStringAsync());
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateUserInfo(object customData)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);

            var url = Consts.GetUrl(Consts.ML_CHANGE_USER_INFO_URL, userId, accessToken);
            var jsonCustomData = JsonConvert.SerializeObject(customData);
            var response = await _httpClient.PutAsync(url, new StringContent(jsonCustomData));
            if (response.StatusCode == System.Net.HttpStatusCode.PartialContent)
            {
                return true;
            }
            return false;
        }

        public async Task<MLAccountBalance> GetUserAccountBalance()
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);

            var url = Consts.GetUrl(Consts.ML_USER_ACCOUNT_BALANCE_URL, userId, accessToken);

            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<MLAccountBalance>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<IList<MLBookmarkItem>> GetBookmarkItems()
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_USER_BOOKMARKS_URL, accessToken);

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<IList<MLBookmarkItem>>(await response.Content.ReadAsStringAsync());

                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            item.ItemInfo = await GetItemDetails(item.item_id/*, new string[] { "title", "price", "thumbnail", "seller_id" }*/);
                        }
                        return result;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MeuMercadoLivreServices.GetBookmarkItems()", ex);
                return null;
            }
        }

        public async Task<IList<PaymentMethod>> ListUserPaymentMethods()
        {
            var url = Consts.GetUrl(Consts.ML_USER_PAYMENT_METHODS_URL, _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID));
            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<IList<PaymentMethod>>(await response.Content.ReadAsStringAsync());
                //var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            return null;
        }
        #endregion

        #region Métodos referentes aos items
        public async Task<MLMyItemsSearchResult> ListMyItems(params KeyValuePair<string, string>[] attributes)
        {
            try
            {
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, userId, accessToken);

                if (attributes != null && attributes.Length > 0)
                {
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        //Concatena com & + chave + = + valor e virgula se for necessario
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributes[i].Key, "=", attributes[i].Value, i < (attributes.Length - 1) ? "," : string.Empty);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLMyItemsSearchResult>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MeuMercadoLivreServices.ListMyItems()", ex);
                return null;
            }
        }

        public async Task<Item> GetItemDetails(string itemId, params KeyValuePair<string, string>[] attributes)
        {
            try
            {
                var parametros = String.Empty;
                for (int i = 0; i < attributes.Length; i++)
                {
                    parametros += attributes[i] + (i < attributes.Length - 1 ? "," : String.Empty);
                }
                var url = Consts.GetUrl(Consts.ML_MY_ITEM_DETAIL, itemId);

                if (attributes != null && attributes.Length > 0)
                {
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        //Concatena com & + chave + = + valor e virgula se for necessario
                        //url = string.Concat(url, "&", attributes[i].Key, "=", attributes[i].Value, i < (attributes.Length - 1) ? "," : string.Empty);
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributes[i].Key, "=", attributes[i].Value, i < (attributes.Length - 1) ? "," : string.Empty);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<Item>(await response.Content.ReadAsStringAsync());

                    //Pega os dados de favorito
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetItemDetails()", ex);
                return null;
            }
        }

        public async Task<MLCategorySearchResult> GetCategoryDetail(string categoryId)
        {
            var url = Consts.GetUrl(Consts.ML_CATEGORY_DETAILS, categoryId);

            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<MLCategorySearchResult>(await response.Content.ReadAsStringAsync());
                return result;
            }
            return null;
        }

        public async Task<MLSearchResult> ListProductsByCategory(string categoryId, int pageIndex = 0, int pageSize = 0)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_CATEGORY, Consts.ML_ID_BRASIL, categoryId);

            if (pageIndex > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, --pageIndex));
            }

#if DEBUG
            Debug.WriteLine("Busca por categoria = " + url);
#endif
            var r = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
            var result = JsonConvert.DeserializeObject<MLSearchResult>(await r.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<MLSearchResult> ListProductsByName(string productName, int pageIndex = 0, int pageSize = 0)
        {
            try
            {
                var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_NAME, Consts.ML_ID_BRASIL, productName);

                if (pageIndex >= 0 && pageSize > 0)
                {
                    url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
                }

#if DEBUG
                Debug.WriteLine("Busca por nome = " + url);
#endif
                var r = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                var result = JsonConvert.DeserializeObject<MLSearchResult>(await r.Content.ReadAsStringAsync());
                return result;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListProductsByName()", ex);
                return null;
            }
        }

        public async Task<ProductQuestionContent> AskQuestion(string question, string productId)
        {
            try
            {
                question = question.Replace(Environment.NewLine, "") + " - Enviado via Meu MercadoLivre Universal para Windows 10";
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_ASK_QUESTION_URL, productId, accessToken);

                //Monta os parametros
                var parametros = new List<KeyValuePair<string, string>>();
                parametros.Add(new KeyValuePair<string, string>("item_id", productId));
                parametros.Add(new KeyValuePair<string, string>("text", question));


                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var jason = JsonConvert.SerializeObject(new { item_id = productId, text = question });

                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<ProductQuestionContent>(result));
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.AskQuestion()", ex);
                return null;
            }
        }

        public async Task<MLProductDescription> GetProductDescrition(string productId)
        {
            try
            {
                var url = Consts.GetUrl(Consts.ML_PRODUCT_DESCRIPTION, productId);
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLProductDescription>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetItemDescription()", ex);
                return null;
            }
        }

        public async Task<Item> ListNewItem(SellItem itemInfo)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_LIST_ITEM, accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = new
                {                   
                    title = itemInfo.Title,
                    price = itemInfo.ProductValue,
                    available_quantity = itemInfo.Quantity,
                    condition = itemInfo.IsNew ?? false ? "new" : "used",
                    description = itemInfo.ProductDescription,
                    buying_mode = itemInfo.BuyingMode,
                    currency_id = itemInfo.CurrencyId,
                    category_id = itemInfo.ProductCategory,
                    listing_type_id = itemInfo.ListType.id

                };
                var json = new StringContent(JsonConvert.SerializeObject(obj));
                

                var response = await _httpClient.PostAsync(url, json);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Item>(result));
                }
                else
                {
                    //Convertemos para o formato de erro
                    var result = await response.Content.ReadAsStringAsync();
                    var error =  await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLErrorRequest>(result));

                    if(error != null)
                    {
                        
                        await AppLogs.WriteLog("MercadoLivreServices.ListNewItem()", "Erro durante a criação do novo item", "");
                        foreach (var item in error.cause)
                        {
                            await AppLogs.WriteLog("     ", item.code + " - " + item.message, "");
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListNewItem()", ex);
                return null;
            }
        }

        /// <summary>
        /// Altera o status do produto
        /// </summary>
        /// <param name="productId">Codigo do produto</param>
        /// <param name="status">Status </param>        
        /// <returns></returns>
        public async Task<bool> ChangeProductStatus(string productId, MLProductStatus status)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_PRODUCT_CHANGE_STATUS, productId, accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var jason = JsonConvert.SerializeObject(new
                {
                    status = status == MLProductStatus.mlpsActive ? "active" : status == MLProductStatus.mlpsPause ? "paused" : "closed"
                });
                var response = await _httpClient.PutAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetProductStatus()", ex);
                return false;
            }
        }

        public async Task<bool> RemoveProduct(string productId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_PRODUCT_REMOVE, productId, accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var jason = JsonConvert.SerializeObject(new
                {
                    deleted = true
                });
                var response = await _httpClient.PutAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.RemoveProduct()", ex);
                return false;
            }
        }

        public async Task<bool> RelistProduct(string productId, int quantity, double price, string listTypeId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_PRODUCT_RELIST, productId, accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                var jason = JsonConvert.SerializeObject(new
                {
                    quantity = quantity,
                    price = price,
                    listing_type_id = listTypeId
                });
                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    //Tentar converter para MLErrorRequest
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.RelistProduct()", ex);
                return false;
            }
        }

        public async Task<bool> ChangeProductAttributes(string productId, dynamic attributes)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_PRODUCT_CHANGE_ATTRIBUTES, productId, accessToken);


                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                var jason = JsonConvert.SerializeObject(attributes);
                var response = await _httpClient.PutAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ChangeProductAttributes()", ex);
                return false;
            }
        }

        public async Task<ShippingCost> GetShippingCost(string productId, string zipCode)
        {
            try
            {
                var url = Consts.GetUrl(Consts.ML_PRODUCT_CALCULATE_SHIPPING_COST, productId, zipCode);
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<ShippingCost>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetShippingCost()", ex);
                return null;
            }
        }

        public async Task<MLProductVisits> GetProductVisits(string productId, DateTime startDate, DateTime finishDate)
        {
            try
            {
                var url = Consts.GetUrl(Consts.ML_PRODUCT_VISITS_URL, productId, startDate.ToString("yyyy-MM-dd"), finishDate.ToString("yyyy-MM-dd"));
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLProductVisits>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetProductVisits()", ex);
                return null;
            }
        }

        #endregion

        #region Métodos referentes a autenticação
        public async Task<bool> RefreshAuthentication()
        {
            var token = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
            var clientSecret = Consts.ML_API_KEY;
            var clientId = Consts.ML_CLIENT_ID;

            var url = Consts.GetUrl(Consts.ML_URL_REFRESH_AUTHENTICATION, clientId, clientSecret, token);
            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<IList<MLAutorizationInfo>>(await response.Content.ReadAsStringAsync());
                //var result = await response.Content.ReadAsStringAsync();
                return true;
            }
            return false;

        }
        #endregion

        #region Métodos referente aos pedidos
        public async Task<MLOrder> ListMyOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, string>[] attributes)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var url = Consts.GetUrl(Consts.ML_MYORDERS_LIST_URL, userId, accessToken);

                if (pageIndex > 0)
                {
                    url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
                }

                if (attributes != null && attributes.Length > 0)
                {
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        //Concatena com & + chave + = + valor e virgula se for necessario
                        url = string.Concat(url, "&", attributes[i].Key, "=", attributes[i].Value, i < (attributes.Length - 1) ? "," : string.Empty);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLOrder>(await response.Content.ReadAsStringAsync());

                    if (result != null)
                    {
                        foreach (var item in result.results)
                        {
                            foreach (var order in item.order_items)
                            {
                                if (order.item != null)
                                {
                                    var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "thumbnail") });
                                    if (thumbmnail != null)
                                        order.item.thumbnail = thumbmnail.thumbnail;
                                }
                            }
                        }
                    }
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListMyOrders()", ex);
                return null;
            }
        }

        public async Task<MLOrder> ListOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, string>[] attributes)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var url = Consts.GetUrl(Consts.ML_ORDERS_LIST, userId, accessToken);

                if (pageIndex >= 0)
                {
                    url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
                }


                if (attributes != null && attributes.Length > 0)
                {
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributes[i].Key, "=", attributes[i].Value);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLOrder>(await response.Content.ReadAsStringAsync());

                    if (result != null)
                    {
                        foreach (var item in result.results)
                        {
                            foreach (var order in item.order_items)
                            {
                                if (order.item != null)
                                {
                                    var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "thumbnail") });
                                    if (thumbmnail != null)
                                        order.item.thumbnail = thumbmnail.thumbnail;
                                }
                            }
                        }
                        return result;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListOrders()", ex);
                return null;
            }
        }

        public async Task<MLOrder> ListRecentOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, string>[] attributes)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var url = Consts.GetUrl(Consts.ML_RECENT_ORDERS_LIST, userId, accessToken);

                if (pageIndex >= 0)
                {
                    url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
                }


                if (attributes != null && attributes.Length > 0)
                {
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributes[i].Key, "=", attributes[i].Value);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLOrder>(await response.Content.ReadAsStringAsync());

                    if (result != null)
                    {
                        foreach (var item in result.results)
                        {
                            foreach (var order in item.order_items)
                            {
                                if (order.item != null)
                                {
                                    var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "thumbnail") });
                                    if (thumbmnail != null)
                                        order.item.thumbnail = thumbmnail.thumbnail;
                                }
                            }
                        }
                        return result;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListOrders()", ex);
                return null;
            }
        }

        public async Task<Feedback> GetOrderFeedback(string orderId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_URL_ORDER_FEEDBACK, orderId, accessToken);


                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<Feedback>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetOrderFeedback()", ex);
                return null;
            }
        }

        #endregion

        public async Task<IList<PaymentMethod>> ListPaymentMethods(string paisId)
        {
            try
            {
                var url = Consts.GetUrl(Consts.ML_PAYMENT_METHODS, Consts.ML_ID_BRASIL);
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<IList<PaymentMethod>>(await response.Content.ReadAsStringAsync());
                    //var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListPaymentMethods()", ex);
                return null;
            }
        }



        public async Task<Shipping> GetShippingDetails(string shipId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_SHIP_DETAILS, shipId, accessToken);


                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<Shipping>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetShippingDetails()", ex);
                return null;
            }
        }


        public async Task<MLMyItemsSearchResult> ListMyItems(string status, int pageIndex = 0, int pageSize = 0)
        {
            try
            {
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, userId, accessToken);

                if (pageIndex > 0)
                {
                    url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, --pageIndex));
                }

                url = String.Concat(url, String.Format("&status={0}", status));

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLMyItemsSearchResult>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListMyItems()", ex);
                return null;
            }
        }


        public async Task<MLMyItemsSearchResult> ListMyItems(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            try
            {
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, userId, accessToken);

                if (pageIndex >= 0 && pageSize > 0)
                {
                    url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
                }

                if (attributesAndFilters != null && attributesAndFilters.Length > 0)
                {
                    for (int i = 0; i < attributesAndFilters.Length; i++)
                    {
                        //Concatena com & + chave + = + valor e virgula se for necessario
                        url = string.Concat(url, "&", attributesAndFilters[i].Key, "=", attributesAndFilters[i].Value, i < (attributesAndFilters.Length - 1) ? "," : string.Empty);
                    }
                }
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLMyItemsSearchResult>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.ListMyItems()", ex);
                return null;
            }
        }


        public async Task<IList<MLListType>> ListTypes(string countryId)
        {
            try
            {
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_LIST_TYPE, countryId);


                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<IList<MLListType>>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MeuMercadoLivreServices.ListTypes()", ex);
                return null;
            }
        }

        public async Task<MLAutorizationInfo> TryRefreshToken()
        {
            try
            {
                var refreshToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
                var url = Consts.GetUrl(Consts.ML_REFRESH_TOKEN_URL, Consts.ML_CLIENT_ID, Consts.ML_API_KEY, refreshToken);


                var response = await _httpClient.PostAsync(url, new StringContent(String.Empty));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLAutorizationInfo>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MeuMercadoLivreServices.TryRefreshToken()", ex);
                return null;
            }
        }

        public async Task<bool> BookmarkItem(string itemId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_PRODUCT_BOOKMARK_URL, accessToken);

                var jason = JsonConvert.SerializeObject(new { item_id = itemId });
                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.BookmarkItem()", ex);
                return false;
            }
        }

        public async Task<bool> RemoveBookmarkItem(string itemId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_PRODUCT_BOOKMARK_REMOVE_URL, itemId, accessToken);

                var jason = JsonConvert.SerializeObject(new { item_id = itemId });
                var response = await _httpClient.DeleteAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.RemoveBookmarkItem()", ex);
                return false;
            }
        }

        public async Task<MLOrderInfo> GetOrderDetail(string orderId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_ORDER_DETAILS, orderId, accessToken);

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLOrderInfo>(await response.Content.ReadAsStringAsync());
                    if (result != null)
                    {
                        foreach (var order in result.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "thumbnail") });
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                        return result;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetOrderDetail()", ex);
                return null;
            }
        }

        public async Task<IReadOnlyList<MLListPrice>> GetListingPrices(string countryId, double productPrice, params KeyValuePair<string, object>[] attributesOrFilters)
        {
            try
            {
                DecimalFormatter formatter = new DecimalFormatter(new string[] { "en-US" }, "US");

                var stringPrice = formatter.Format(productPrice);
                var url = Consts.GetUrl(Consts.ML_LIST_PRICES, countryId, stringPrice);

                if (attributesOrFilters != null && attributesOrFilters.Length > 0)
                {
                    for (int i = 0; i < attributesOrFilters.Length; i++)
                    {
                        url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesOrFilters[i].Key, "=", attributesOrFilters[i].Value, i < (attributesOrFilters.Length - 1) ? "," : string.Empty);
                    }
                }

                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<IReadOnlyList<MLListPrice>>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.GetListingPrices()", ex);
                return null;
            }
        }

        public async Task<bool> RevokeAccess()
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var url = String.Format(Consts.ML_REVOKE_LOGIN, userId, Consts.ML_CLIENT_ID, accessToken);
                var r = await _httpClient.DeleteAsync(url);

                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var s = await r.Content.ReadAsStringAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.RevokeAccess()", ex);
                return false;
            }
        }

        public async Task<IReadOnlyList<MLListType>> GetAvailableUpgrades(string itemId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = String.Format(Consts.ML_LIST_ITEM_UPGRADES, itemId, accessToken);
                var r = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);

                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var s = await r.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<IReadOnlyList<MLListType>>(s));
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.RevokeAccess()", ex);
                return null;
            }
        }

        public async Task<bool> ChangeItemListType(string itemId, string id_type)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_ITEM_CHANGE_LIST_TYPE, itemId, accessToken);

                var jason = JsonConvert.SerializeObject(new { id = id_type });
                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.RemoveBookmarkItem()", ex);
                return false;
            }
        }


        public async Task<bool> SendSellerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message, MLSellerRatingReason reason, bool restockItem = false, bool hasSellerRefundMoney = false)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_URL_ORDER_FEEDBACK, orderId, accessToken);


                var jason = string.Empty;
                if (fulfilled)
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message
                    });
                }
                else
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message,
                        reason = Enum.GetName(typeof(MLSellerRatingReason), reason),
                        restock_item = restockItem,
                        has_seller_refunded_money = hasSellerRefundMoney
                    });
                }
                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.SendSellerOrderFeedback()", ex);
                return false;
            }
        }

        public async Task<bool> SendBuyerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message, MLBuyerRatingReason reason)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_URL_ORDER_FEEDBACK, orderId, accessToken);


                var jason = string.Empty;
                if (fulfilled)
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message
                    });
                }
                else
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message,
                        reason = Enum.GetName(typeof(MLBuyerRatingReason), reason)
                    });
                }
                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.SendBuyerOrderFeedback()", ex);
                return false;
            }
        }
        public async Task<MLImage> UploadProductImage(ProductImage image)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_UPLOAD_IMAGE, accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StringContent("file");
                form.Add(content, "file");
                var file = await StorageFile.GetFileFromPathAsync(image.LocalPath);
                var stream = await file.OpenStreamForReadAsync();
                content = new StreamContent(stream);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = file.Name
                };

                form.Add(content);
                var response = await _httpClient.PostAsync(url, form);

                if(response.StatusCode == System.Net.HttpStatusCode.OK ||  response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLImage>(result));
                }
                return null;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.UploadProductImage()", ex);
                return null;
            }
            

        }
        public async Task<bool> AddPicture(string pictureId, string itemId)
        {
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_ITEM_POST_IMAGE, itemId, accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = new
                {
                    id = pictureId
                };
                var json = new StringContent(JsonConvert.SerializeObject(obj));

                var response = await _httpClient.PostAsync(url, json);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MercadoLivreServices.AddPicture()", ex);
                return false;
            }
        }
    }
}

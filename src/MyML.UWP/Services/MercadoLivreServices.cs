using MyML.UWP.Models;
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
            return await BaseServices<IList<MLCategorySearchResult>>
                .GetAsync(String.Format(Consts.ML_URL_CATEGORIAS, Consts.ML_ID_BRASIL)).ConfigureAwait(false);
        }

        #region Métodos referente as questões
        public async Task<Models.Mercadolivre.MLQuestionResultSearch> ListQuestions(params KeyValuePair<string, object>[] attributesAndFilters)
        {
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);


            var url = Consts.GetUrl(Consts.ML_QUESTIONS_URL, userId, accessToken);
            var result = await BaseServices<MLQuestionResultSearch>
                .GetAsync(url, attributesAndFilters).ConfigureAwait(false);

            return result;
        }

        public async Task<bool> AnswerQuestion(string questionId, string content)
        {
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
            var response = await _httpClient.PostAsync(url, json).ConfigureAwait(false);
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


            var response = await _httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<ProductQuestion> ListQuestionsByProduct(string itemId, int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_QUESTIONS, itemId);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            return await BaseServices<ProductQuestion>.GetAsync(url, attributesAndFilters).ConfigureAwait(false);
        }

        public async Task<ProductQuestionContent> GetQuestionDetails(string questionId, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);


            var url = Consts.GetUrl(Consts.ML_QUESTIONS_DETAIL_URL, questionId);

            return await BaseServices<ProductQuestionContent>.GetAsync(url, attributesAndFilters).ConfigureAwait(false);
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
        public async Task<MLUserInfoSearchResult> GetUserInfo(string userId, params KeyValuePair<string, object>[] attributesOrFilters)
        {
            var url = Consts.GetUrl(Consts.ML_USER_INFO_URL, userId);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            url = string.Concat(url, "?", accessToken);
            return await BaseServices<MLUserInfoSearchResult>.GetAsync(url, attributesOrFilters).ConfigureAwait(false);
        }

        public async Task<MLUserInfoSearchResult> GetUserProfile()
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.ML_USER_PROFILE, accessToken);
            return await BaseServices<MLUserInfoSearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<IList<UserAddress>> GetUserAddress(int userId)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.ML_USER_ADDRESSES, userId, accessToken);
            return await BaseServices<IList<UserAddress>>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<bool> UpdateUserInfo(object customData)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);

            var url = Consts.GetUrl(Consts.ML_CHANGE_USER_INFO_URL, userId, accessToken);
            var jsonCustomData = JsonConvert.SerializeObject(customData);
            var response = await _httpClient.PutAsync(url, new StringContent(jsonCustomData)).ConfigureAwait(false);
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
            return await BaseServices<MLAccountBalance>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<IList<MLBookmarkItem>> GetBookmarkItems()
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_USER_BOOKMARKS_URL, accessToken);
            var items = await BaseServices<IList<MLBookmarkItem>>.GetAsync(url).ConfigureAwait(false);
            if (items != null)
                foreach (var item in items)
                {
                    item.ItemInfo = await GetItemDetails(item.item_id, new KeyValuePair<string, object>("attributes", "id,thumbnail,price,title")).ConfigureAwait(false);
                }
            return items;
        }

        public async Task<IList<PaymentMethod>> ListUserPaymentMethods()
        {
            var url = Consts.GetUrl(Consts.ML_USER_PAYMENT_METHODS_URL, _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID));
            return await BaseServices<IList<PaymentMethod>>.GetAsync(url).ConfigureAwait(false);
        }
        #endregion

        #region Métodos referentes aos items
        public async Task<MLMyItemsSearchResult> ListMyItems(params KeyValuePair<string, object>[] attributes)
        {
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, userId, accessToken);
            return await BaseServices<MLMyItemsSearchResult>.GetAsync(url, attributes).ConfigureAwait(false);
        }

        public async Task<Item> GetItemDetails(string itemId, params KeyValuePair<string, object>[] attributes)
        {
            var url = Consts.GetUrl(Consts.ML_MY_ITEM_DETAIL, itemId);
            return await BaseServices<Item>.GetAsync(url, attributes).ConfigureAwait(false);
        }

        public async Task<MLCategorySearchResult> GetCategoryDetail(string categoryId)
        {
            var url = Consts.GetUrl(Consts.ML_CATEGORY_DETAILS, categoryId);
            return await BaseServices<MLCategorySearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLSearchResult> ListProductsByCategory(string categoryId, int pageIndex = 0, int pageSize = 0)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_CATEGORY, Consts.ML_ID_BRASIL, categoryId);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, --pageIndex));
            }
            return await BaseServices<MLSearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLSearchResult> ListProductsByName(string productName, int pageIndex = 0, int pageSize = 0)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_NAME, Consts.ML_ID_BRASIL, productName);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
            }
            return await BaseServices<MLSearchResult>.GetAsync(url).ConfigureAwait(false);
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
                await AppLogs.WriteError("MercadoLivreServices.AskQuestion()", ex);
                return null;
            }
        }

        public async Task<MLProductDescription> GetProductDescrition(string productId)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_DESCRIPTION, productId);
            return await BaseServices<MLProductDescription>.GetAsync(url).ConfigureAwait(false);
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
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Item>(result));
                }
                else
                {
                    //Convertemos para o formato de erro
                    var error = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLErrorRequest>(result));

                    if (error?.cause != null)
                    {

                        await AppLogs.WriteLog("MercadoLivreServices.ListNewItem()", "Erro durante a criação do novo item", "");
                        if (error.cause != null)
                            foreach (var item in error.cause)
                            {
                                await AppLogs.WriteLog("     ", item.code + " - " + item.message, "");
                            }
                    }

                    await AppLogs.WriteWarning("MercadoLivreServices.ListNewItem()", result);
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ListNewItem()", ex);
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
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                await AppLogs.WriteWarning("MercadoLivreServices.GetProductStatus()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.GetProductStatus()", ex);
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
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }

                await AppLogs.WriteWarning("MercadoLivreServices.RemoveProduct()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RemoveProduct()", ex);
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
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    //Tentar converter para MLErrorRequest
                }
                await AppLogs.WriteWarning("MercadoLivreServices.RelistProduct()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RelistProduct()", ex);
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
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                await AppLogs.WriteWarning("MercadoLivreServices.ChangeProductAttributes()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ChangeProductAttributes()", ex);
                return false;
            }
        }

        public async Task<ShippingCost> GetShippingCost(string productId, string zipCode)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_CALCULATE_SHIPPING_COST, productId, zipCode);
            return await BaseServices<ShippingCost>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLProductVisits> GetProductVisits(string productId, DateTime startDate, DateTime finishDate)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_VISITS_URL, productId, startDate.ToString("yyyy-MM-dd"), finishDate.ToString("yyyy-MM-dd"));
            return await BaseServices<MLProductVisits>.GetAsync(url).ConfigureAwait(false);
        }

        #endregion

        #region Métodos referentes a autenticação
        public async Task<bool> RefreshAuthentication()
        {
            var token = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
            var clientSecret = Consts.ML_API_KEY;
            var clientId = Consts.ML_CLIENT_ID;

            var url = Consts.GetUrl(Consts.ML_URL_REFRESH_AUTHENTICATION, clientId, clientSecret, token);
            var result = await BaseServices<MLAutorizationInfo>.GetAsync(url).ConfigureAwait(false);
            return !String.IsNullOrWhiteSpace(result?.Access_Token);
        }
        #endregion

        #region Métodos referente aos pedidos
        public async Task<MLOrder> ListMyOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlMyordersListUrl, userId, accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);
            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") }).ConfigureAwait(false);
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<MLOrder> ListOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlOrdersList, userId, accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);
            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>("attributes", "thumbnail")).ConfigureAwait(false);
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<MLOrder> ListRecentOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {

            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlRecentOrdersList, userId, accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);

            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") });
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<Feedback> GetOrderFeedback(string orderId)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.MlUrlOrderFeedback, orderId, accessToken);
            return await BaseServices<Feedback>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLOrder> ListArchivedSellerOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlUrlOrderSellerArchived, userId, accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url).ConfigureAwait(false);

            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") });
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<MLOrder> ListArchivedBuyerOrders(int pageIndex = 0, int pageSize = 0,
            params KeyValuePair<string, object>[] attributes)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlUrlOrderBuyerArchived, userId, accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }
            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);

            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail =
                                    await
                                        GetItemDetails(order.item.id,
                                            new KeyValuePair<string, object>[]
                                            {new KeyValuePair<string, object>("attributes", "thumbnail")})
                                            .ConfigureAwait(false);
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        #endregion

        public async Task<IList<PaymentMethod>> ListPaymentMethods(string paisId)
        {
            var url = Consts.GetUrl(Consts.ML_PAYMENT_METHODS, Consts.ML_ID_BRASIL);
            return await BaseServices<IList<PaymentMethod>>.GetAsync(url).ConfigureAwait(false);
        }


        public async Task<Shipping> GetShippingDetails(string shipId)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.ML_SHIP_DETAILS, shipId, accessToken);
            return await BaseServices<Shipping>.GetAsync(url).ConfigureAwait(false);
        }


        public async Task<MLMyItemsSearchResult> ListMyItems(string status, int pageIndex = 0, int pageSize = 0)
        {
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, userId, accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, --pageIndex));
            }

            url = String.Concat(url, String.Format("&status={0}", status));
            return await BaseServices<MLMyItemsSearchResult>.GetAsync(url).ConfigureAwait(false);
        }


        public async Task<MLMyItemsSearchResult> ListMyItems(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, userId, accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
            }

            return await BaseServices<MLMyItemsSearchResult>.GetAsync(url, attributesAndFilters).ConfigureAwait(false);
        }


        public async Task<IList<MLListType>> ListTypes(string countryId)
        {
            var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_LIST_TYPE, countryId);
            return await BaseServices<IList<MLListType>>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLAutorizationInfo> TryRefreshToken()
        {
            try
            {
                var refreshToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
                var url = Consts.GetUrl(Consts.ML_REFRESH_TOKEN_URL, Consts.ML_CLIENT_ID, Consts.ML_API_KEY, refreshToken);


                var response = await _httpClient.PostAsync(url, new StringContent(String.Empty)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLAutorizationInfo>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MeuMercadoLivreServices.TryRefreshToken()", ex);
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
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.BookmarkItem()", ex);
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
                var response = await _httpClient.DeleteAsync(url).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RemoveBookmarkItem()", ex);
                return false;
            }
        }

        public async Task<MLOrderInfo> GetOrderDetail(string orderId)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.MlOrderDetails, orderId, accessToken);
            var result = await BaseServices<MLOrderInfo>.GetAsync(url).ConfigureAwait(false);
            if (result?.order_items != null)
            {
                foreach (var order in result.order_items)
                {
                    if (order.item != null)
                    {
                        var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") }).ConfigureAwait(false);
                        if (thumbmnail != null)
                            order.item.thumbnail = thumbmnail.thumbnail;
                    }
                }
            }
            return result;
        }

        public async Task<IReadOnlyList<MLListPrice>> GetListingPrices(string countryId, double productPrice, params KeyValuePair<string, object>[] attributesOrFilters)
        {
            DecimalFormatter formatter = new DecimalFormatter(new string[] { "en-US" }, "US");

            var stringPrice = formatter.Format(productPrice);
            var url = Consts.GetUrl(Consts.ML_LIST_PRICES, countryId, stringPrice);

            return await BaseServices<IReadOnlyList<MLListPrice>>.GetAsync(url, attributesOrFilters).ConfigureAwait(false);
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
                await AppLogs.WriteError("MercadoLivreServices.RevokeAccess()", ex);
                return false;
            }
        }

        public async Task<IReadOnlyList<MLListType>> GetAvailableUpgrades(string itemId)
        {
            var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = String.Format(Consts.ML_LIST_ITEM_UPGRADES, itemId, accessToken);
            return await BaseServices<IReadOnlyList<MLListType>>.GetAsync(url).ConfigureAwait(false);
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
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RemoveBookmarkItem()", ex);
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
                var url = Consts.GetUrl(Consts.MlUrlOrderFeedback, orderId, accessToken);


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
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.SendSellerOrderFeedback()", ex);
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
                var url = Consts.GetUrl(Consts.MlUrlOrderFeedback, orderId, accessToken);


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
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.SendBuyerOrderFeedback()", ex);
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

                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLImage>(result));
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.UploadProductImage()", ex);
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
                await AppLogs.WriteError("MercadoLivreServices.AddPicture()", ex);
                return false;
            }
        }

        public async Task<MLErrorRequest> ValidateNewItem(SellItem itemInfo)
        {
            //
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_VALIDATE_NEW_ITEM, accessToken);

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


                var response = await _httpClient.PostAsync(url, json).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return null;
                }
                else
                {
                    //Convertemos para o formato de erro
                    var error = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLErrorRequest>(result)).ConfigureAwait(false);

                    if (error != null)
                    {

                        await AppLogs.WriteLog("MercadoLivreServices.ValidateNewItem()", "Erro durante a criação do novo item", "").ConfigureAwait(false);
                        if (error.cause != null)
                            foreach (var item in error.cause)
                            {
                                await AppLogs.WriteLog("     ", item.code + " - " + item.message, "");
                            }
                    }

                    await AppLogs.WriteWarning("MercadoLivreServices.ValidateNewItem()", result);
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ValidateNewItem()", ex);
                return null;
            }
        }


        public async Task<bool> ChangeProductDescription(string productId, string text)
        {
            //
            try
            {
                var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_CHANGE_PRODUCT_DESCRIPTION, productId, accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = new
                {
                    text = text
                };
                var json = new StringContent(JsonConvert.SerializeObject(obj));


                var response = await _httpClient.PutAsync(url, json).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    //Convertemos para o formato de erro
                    var error = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLErrorRequest>(result)).ConfigureAwait(false);

                    if (error != null)
                    {

                        await AppLogs.WriteLog("MercadoLivreServices.ChangeProductDescription()", "Erro durante a criação do novo item", "").ConfigureAwait(false);
                        if (error.cause != null)
                            foreach (var item in error.cause)
                            {
                                await AppLogs.WriteLog("     ", item.code + " - " + item.message, "");
                            }
                    }

                    await AppLogs.WriteWarning("MercadoLivreServices.ListNewItem()", result);
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ChangeProductDescription()", ex);
                return false;
            }
        }

        public async Task<MLSearchResult> ListProductsByUser(string userId, int pageIndex = 0, int pageSize = 0)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_USER, Consts.ML_ID_BRASIL, userId);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = String.Concat(url, String.Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
            }
            return await BaseServices<MLSearchResult>.GetAsync(url).ConfigureAwait(false);
        }
    }
}

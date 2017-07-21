using System.Diagnostics;

namespace MyML.UWP.Models
{
    public static class Consts
    {
        //public const string CONFIG_LICENSE_REMOVE_ADS = "CONFIG_LICENSE_REMOVE_ADS";
        internal static readonly string CONFIG_REMOVE_ADS_KEY = "ML_REMOVER_ANUNCIO";
        internal static readonly string BACKGROUND_TASKNAME_QUESTIONS = "AnswerQuestionTask";


        public const string CONFIG_KEY_EXECUCOES = "ml_executions";
        public const string ML_NOTIFICATION_EXPIRES = "ml_notification_expires";
        public const string ML_CONFIG_KEY_USER_ID = "ml_user_id";
        public const string ML_CONFIG_KEY_EXPIRES = "ml_expires";
        public const string ML_CONFIG_KEY_ACCESS_TOKEN = "ml_access_token";
        public const string ML_CONFIG_KEY_REFRESH_TOKEN = "ml_refresh_token";
        public const string ML_CONFIG_KEY_LOGIN_DATE = "ml_login_date";

        public const string ML_API_BASE_URL = "https://api.mercadolibre.com";
        public const string MP_API_BASE_URL = "https://api.mercadopago.com";
        
        //public const string ML_ID_BRASIL = "MLB";
        public const string ML_URL_CATEGORIAS = "https://api.mercadolibre.com/sites/{0}/categories";
        
        
        public const string ML_API_KEY = "1pW3zr833brws5ePUbtfcQc52xzq1Ocq";
        public const string ML_RETURN_URL = "https://www.miniapps.com.br/api/mercadolivre/login";
        public const string ML_CLIENT_ID = "8765232316929095";
        public const string ML_REFRESH_TOKEN_URL = "https://api.mercadolibre.com/oauth/token?grant_type=refresh_token&client_id={0}&client_secret={1}&refresh_token={2}";
        public const string ML_URL_LIST_SITES = "https://api.mercadolibre.com/sites";

        /// <summary>
        /// Parâmetros: UserId, ClientId, AccessToken
        /// </summary>
        public const string ML_REVOKE_LOGIN = "https://api.mercadolibre.com/users/{0}/applications/{1}?access_token={2}";

        #region Urls referente as questões
        public const string ML_QUESTIONS_DETAIL_URL = "https://api.mercadolibre.com/questions/{0}";

        /// <summary>
        /// Parametros: UserId, Access_Token
        /// </summary>
        public const string ML_QUESTIONS_URL = "https://api.mercadolibre.com/questions/search?status=unanswered&seller={0}&access_token={1}";
        //public const string ML_QUESTIONS_URL = "https://api.mercadolibre.com/my/received_questions/search?access_token={0}";
        

        /// <summary>
        /// Parametros: Access_Token
        /// </summary>
        public const string ML_QUESTIONS_ANSWER_URL = "https://api.mercadolibre.com/answers?access_token={0}";

        /// <summary>
        /// Parametros: ProductId, Access_token
        /// </summary>
        public const string ML_ASK_QUESTION_URL = "https://api.mercadolibre.com/questions/{0}?access_token={1}";

        /// <summary>
        /// Parametros: QuestionId, Access_token
        /// </summary>
        public const string ML_REMOVE_QUESTION_URL = "https://api.mercadolibre.com/questions/{0}?access_token={1}";

        /// <summary>
        /// Parametros: AccessToken
        /// </summary>
        public const string ML_QUESTIONS_LIST = "https://api.mercadolibre.com/my/received_questions/search?access_token={0}";

        
        
        #endregion

        #region Urls referente aos meus items
        /// <summary>
        /// Parâmetros: UserId, Access_Token
        /// </summary>
        public const string ML_MY_ITEMS_URL = "https://api.mercadolibre.com/users/{0}/items/search?access_token={1}";

        /// <summary>
        /// Parametros: Access_Token
        /// </summary>
        public const string ML_LIST_ITEM = "https://api.mercadolibre.com/items?access_token={0}";

        /// <summary>
        /// Parametros: ItemId, AccessToken
        /// </summary>
        public const string ML_ITEM_POST_IMAGE = "https://api.mercadolibre.com/items/{0}/pictures?access_token={1}";

        /// <summary>
        /// Parametros: AccessToken
        /// </summary>
        public const string ML_UPLOAD_IMAGE = "https://api.mercadolibre.com/pictures?access_token={0}";

        /// <summary>
        /// 
        /// </summary>
        public const string ML_VALIDATE_NEW_ITEM = "https://api.mercadolibre.com/items/validate?access_token={0}";
        #endregion

        #region Urls referente aos dados do usuario
        /// <summary>
        /// Parametros UserId
        /// </summary>
        public const string ML_USER_INFO_URL = "https://api.mercadolibre.com/users/{0}";

        /// <summary>
        /// Parametros: UserId, AccessToken
        /// </summary>
        public const string ML_USER_ADDRESSES = "https://api.mercadolibre.com/users/{0}/addresses?access_token={1}";

        /// <summary>
        /// Parametros: AccessToken
        /// </summary>
        public const string ML_USER_PROFILE = "https://api.mercadolibre.com/users/me?access_token={0}";

        /// <summary>
        /// Parametros: UserId, AccessToken
        /// </summary>
        public const string ML_CHANGE_USER_INFO_URL = "https://api.mercadolibre.com/users/{0}?access_token={1}";

        /// <summary>
        /// Parametros: UserId, AccessToken
        /// </summary>
        public const string ML_USER_ACCOUNT_BALANCE_URL = "https://api.mercadolibre.com/users/{0}/mercadopago_account/balance?access_token={1}";

        /// <summary>
        /// Parametros: AccessToken
        /// </summary>
        public const string ML_USER_BOOKMARKS_URL = "https://api.mercadolibre.com/users/me/bookmarks?access_token={0}";

        /// <summary>
        /// Parametros: UserId
        /// </summary>
        public const string ML_USER_PAYMENT_METHODS_URL = "https://api.mercadolibre.com/users/{0}/accepted_payment_methods";
        #endregion

        #region Urls referente aos produtos
        /// <summary>
        /// Parametros: ItemId
        /// </summary>
        public const string ML_MY_ITEM_DETAIL = "https://api.mercadolibre.com/items/{0}";

        /// <summary>
        /// Parametros: CategoryId 
        /// </summary>
        public const string ML_CATEGORY_DETAILS = "https://api.mercadolibre.com/categories/{0}";

        /// <summary>
        /// Parametros: UserId, Access_Token
        /// </summary>
        public const string ML_MY_PRODUCTS_LIST = "https://api.mercadolibre.com/users/{0}/items/search?access_token={1}";

        /// <summary>
        /// Parametros: LojaId, Query
        /// </summary>
        public const string ML_PRODUCTS_BY_NAME = "https://api.mercadolibre.com/sites/{0}/search?q={1}&adult_content={2}";

        /// <summary>
        /// Parametros: PaisId, UserId
        /// </summary>
        public const string ML_PRODUCTS_BY_USER = "https://api.mercadolibre.com/sites/{0}/search?seller_id={1}";

        /// <summary>
        /// Parametros: LojaId, CategoryId
        /// </summary>
        public const string ML_PRODUCTS_BY_CATEGORY = "https://api.mercadolibre.com/sites/{0}/search?category={1}";

        /// <summary>
        /// Parametros: ItemId
        /// </summary>
        public const string ML_PRODUCT_DESCRIPTION = "https://api.mercadolibre.com/items/{0}/description";

        /// <summary>
        /// Parametros: ItemId, AccessToken
        /// </summary>
        public const string ML_CHANGE_PRODUCT_DESCRIPTION = "https://api.mercadolibre.com/items/{0}/description?access_token={1}";

        /// <summary>
        /// Parametros: ItemId
        /// </summary>
        public const string ML_PRODUCT_QUESTIONS = "https://api.mercadolibre.com/questions/search?item_id={0}";

        /// <summary>
        /// Parametros: ItemId, AccessToken
        /// </summary>
        public const string ML_PRODUCT_CHANGE_STATUS = "https://api.mercadolibre.com/items/{0}?access_token={1}";

        /// <summary>
        /// Parametros: ItemId, AccessToken
        /// </summary>
        public const string ML_PRODUCT_REMOVE = "https://api.mercadolibre.com/items/{0}?access_token={1}";

        /// <summary>
        /// Parametros: ItemId, AccessToken
        /// </summary>
        public const string ML_PRODUCT_RELIST = "https://api.mercadolibre.com/items/{0}/relist?access_token={1}";

        /// <summary>
        /// Parametros: ItemId, AccessToken
        /// </summary>
        public const string ML_PRODUCT_CHANGE_ATTRIBUTES = "https://api.mercadolibre.com/items/{0}?access_token={1}";

        /// <summary>
        /// Parametros: ItemId, ZipCode
        /// </summary>
        public const string ML_PRODUCT_CALCULATE_SHIPPING_COST = "https://api.mercadolibre.com/items/{0}/shipping_options?zip_code={1}";

        /// <summary>
        /// Parametros: ProductId, Date1 (yyyy-mm-dd), Date2 (yyyy-mm-dd)
        /// </summary>
        public const string ML_PRODUCT_VISITS_URL = "https://api.mercadolibre.com/items/{0}/visits/?date_from={1}&date_to={2}";

        /// <summary>
        /// Parametros: AccessToken
        /// </summary>
        public const string ML_PRODUCT_BOOKMARK_URL = "https://api.mercadolibre.com/users/me/bookmarks?access_token={0}";

        /// <summary>
        /// Parametros: ProductId, AccessToken
        /// </summary>
        public const string ML_PRODUCT_BOOKMARK_REMOVE_URL = "https://api.mercadolibre.com/users/me/bookmarks/{0}?access_token={1}";
        

        #endregion

        #region Urls referente a forma de pagamento
        /// <summary>
        /// Parametros Site_Id
        /// </summary>
        public const string ML_PAYMENT_METHODS = "https://api.mercadolibre.com/sites/{0}/payment_methods";
        #endregion

        #region Urls referente aos pedidos
        /// <summary>
        /// Parâmetros UserId, Access Token
        /// </summary>
        public const string MlOrdersList = "https://api.mercadolibre.com/orders/search?seller={0}&access_token={1}";
        public const string MlRecentOrdersList = "https://api.mercadolibre.com/orders/search/recent/?seller={0}&access_token={1}";

        /// <summary>
        /// 
        /// </summary>
        public const string MlOrderDetails = "https://api.mercadolibre.com/orders/{0}?access_token={1}";
        

        /// <summary>
        /// Parâmetros UserId, Access Token
        /// </summary>
        public const string MlMyordersListUrl = "https://api.mercadolibre.com/orders/search?buyer={0}&access_token={1}";

        /// <summary>
        /// Parametros OrderId, AccessToken
        /// </summary>
        public const string MlUrlOrderFeedback = "https://api.mercadolibre.com/orders/{0}/feedback?access_token={1}";

        /// <summary>
        /// Vendas arquivadas
        /// Parametros: SellerId, AccessToken
        /// </summary>
        public const string MlUrlOrderSellerArchived = "https://api.mercadolibre.com/orders/search/archived?seller={0}&access_token={1}";

        /// <summary>
        /// Compras arquivadas
        /// Parametros: BuyerId, AccessToken
        /// </summary>
        public const string MlUrlOrderBuyerArchived = "https://api.mercadolibre.com/orders/search/archived?buyer={0}&access_token={1}";


        #endregion

        #region Urls referente a entrega
        /// <summary>
        /// Parametros ShipId, AccessToken
        /// </summary>
        public const string ML_SHIP_DETAILS = "https://api.mercadolibre.com/shipments/{0}?access_token={1}";
        
        #endregion

        public const string ML_LIST_TYPE = "https://api.mercadolibre.com/sites/{0}/listing_types";
        public const string ML_LIST_PRICES = "https://api.mercadolibre.com/sites/{0}/listing_prices?price={1}";

        /// <summary>
        /// Parametros: ItemId, AccessToken
        /// </summary>
        public const string ML_ITEM_CHANGE_LIST_TYPE = "https://api.mercadolibre.com/items/{0}/listing_type?access_token={1}";


        /// <summary>
        /// Parametros: ClientId, ClienteSecret,Code,Redirect URI
        /// </summary>
        //public const string ML_AUTORIZATION_URL = "https://api.mercadolibre.com/oauth/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&redirect_uri={3}";
        public const string ML_AUTORIZATION_URL = "https://api.mercadolibre.com/oauth/token";

        /// <summary>
        /// Parametros client_id, redirect_uri
        /// </summary>
        public const string ML_URL_AUTHENTICATION = "https://auth.mercadolivre.com.br/authorization?response_type=code&client_id={0}&redirect_uri={1}";

        /// <summary>
        /// Parametros ClientId, ClientSecret, RefreshToken
        /// </summary>
        public const string ML_URL_REFRESH_AUTHENTICATION = "https://api.mercadolibre.com/oauth/token?grant_type=refresh_token&client_id={0}&client_secret={1}&refresh_token={2}";

        /// <summary>
        /// Parametros ItemId, AccessToken
        /// </summary>
        public const string ML_LIST_ITEM_UPGRADES = "https://api.mercadolibre.com/items/{0}/available_upgrades?access_token={1}";

        /// <summary>
        /// Parametros PaisId
        /// </summary>
        public const string MlUrlDestaquesHome = "https://api.mercadolibre.com/sites/{0}/featured_items/HP";
        
        /// <summary>
        /// Parametros PaisId, CategoriaId
        /// </summary>
        public const string MlUrlDestaquesCategoria = "https://api.mercadolibre.com/sites/{0}/featured_items/HO-{1}";

        public static string GetUrl(string url, params object[] parameters)        
        {
            string result = null;
            result = string.Format(url, parameters);

#if DEBUG
            Debug.WriteLine($"URL REQUISITADA={result}");
#endif
            return result;
        }
    }
}

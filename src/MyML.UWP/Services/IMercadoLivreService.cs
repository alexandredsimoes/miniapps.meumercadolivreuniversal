using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Services
{
    public interface IMercadoLivreService
    {
        //Task<bool> IsAuthenticated();
        Task<IList<MLCategorySearchResult>> ListCategories(string paisId);
        Task<MLQuestionResultSearch> ListQuestions(params KeyValuePair<string,object>[] attributesAndFilters);
        Task<ProductQuestionContent> GetQuestionDetails(string questionId, params KeyValuePair<string, object>[] attributesAndFilters);
        Task<MLMyItemsSearchResult> ListMyItems(params KeyValuePair<string, object>[] attributes);
        Task<bool> AnswerQuestion(string questionId, string content);        
        Task<bool> RemoveQuestion(string questionId);
        Task<MLUserInfoSearchResult> GetUserInfo(string userId, params KeyValuePair<string, object>[] attributesOrFilters);
        Task<MLUserInfoSearchResult> GetUserProfile();
        Task<MLAccountBalance> GetUserAccountBalance();
        Task<IList<UserAddress>> GetUserAddress(int userId);
        Task<bool> UpdateUserInfo(object customData);
        Task<Item> GetItemDetails(string itemId, params KeyValuePair<string, object>[] attributes);
        Task<MLCategorySearchResult> GetCategoryDetail(string categoryId);
        Task<MLSearchResult> ListProductsByCategory(string categoryId, int pageIndex = 0, int pageSize = 0);
        Task<MLSearchResult> ListProductsByName(string productName, int pageIndex = 0, int pageSize = 0);
        Task<ProductQuestionContent> AskQuestion(string question, string productId);
        Task<IList<PaymentMethod>> ListPaymentMethods(string paisId);
        Task<IList<PaymentMethod>> ListUserPaymentMethods();
        Task<ProductQuestion> ListQuestionsByProduct(string itemId, int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters);
        Task<MLOrder> ListOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string,object>[] attributes);
        Task<MLOrder> ListMyOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes);
        Task<MLOrderInfo> GetOrderDetail(string orderId);
        Task<bool> RefreshAuthentication();
        Task<Shipping> GetShippingDetails(string shipId);
        Task<Feedback> GetOrderFeedback(string orderId);
        Task<MLProductDescription> GetProductDescrition(string productId);
        Task<MLMyItemsSearchResult> ListMyItems(string status, int pageIndex = 0, int pageSize = 0);
        Task<MLMyItemsSearchResult> ListMyItems(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters);
        Task<IList<MLListType>> ListTypes(string countryId);
        Task<bool> ChangeProductStatus(string productId, MLProductStatus status);
        Task<bool> RemoveProduct(string productId);
        Task<bool> RelistProduct(string productId, int quantity, double price, string listTypeId);
        Task<bool> ChangeProductAttributes(string productId, dynamic attributes);
        Task<ShippingCost> GetShippingCost(string productId, string zipCode);
        Task<IList<MLBookmarkItem>> GetBookmarkItems();
        Task<bool> BookmarkItem(string itemId);
        Task<bool> RemoveBookmarkItem(string itemId);
        Task<MLAutorizationInfo> TryRefreshToken();
        Task<MLProductVisits> GetProductVisits(string productId, DateTime startDate, DateTime finishDate);
        Task<IReadOnlyList<MLListPrice>> GetListingPrices(string countryId, double productPrice, params KeyValuePair<string, object>[] attributesOrFilters);
        Task<Item> ListNewItem(SellItem itemInfo);
        Task<bool> RevokeAccess();
        //Task<MLQuestionResultSearch> ListQuestions(KeyValuePair<string, object>[] attributesOrFilters);
        Task<IReadOnlyList<MLListType>> GetAvailableUpgrades(string itemId);
        Task<bool> ChangeItemListType(string itemId, string id_type);
        Task<bool> SendSellerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message, MLSellerRatingReason reason,
            bool restockItem = false, bool hasSellerRefundMoney = false);
        Task<bool> SendBuyerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message, MLBuyerRatingReason reason);
        Task<MLOrder> ListRecentOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes);
        Task<MLImage> UploadProductImage(ProductImage image);
        Task<bool> AddPicture(string pictureId, string itemId);
        Task<MLErrorRequest> ValidateNewItem(SellItem itemInfo);

        Task<MLOrder> ListArchivedSellerOrders(int pageIndex = 0, int pageSize = 0,
            params KeyValuePair<string, object>[] attributes);

        Task<MLOrder> ListArchivedBuyerOrders(int pageIndex = 0, int pageSize = 0,
            params KeyValuePair<string, object>[] attributes);

        Task<bool> ChangeProductDescription(string productId, string text);

        Task<MLSearchResult> ListProductsByUser(string userId, int pageIndex = 0, int pageSize = 0);
    }
}

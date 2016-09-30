using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;

namespace MyML.UWP.Services
{
    public class DesignMercadoLivreServices : IMercadoLivreService
    {
        public Task<IList<MLCategorySearchResult>> ListCategories(string paisId)
        {
            throw new NotImplementedException();
        }

        public Task<MLQuestionResultSearch> ListQuestions(params KeyValuePair<string, object>[] attributesAndFilters)
        {
            throw new NotImplementedException();
        }

        public Task<ProductQuestionContent> GetQuestionDetails(string questionId, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            throw new NotImplementedException();
        }

        public Task<MLMyItemsSearchResult> ListMyItems(params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnswerQuestion(string questionId, string content)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveQuestion(string questionId)
        {
            throw new NotImplementedException();
        }

        public Task<MLUserInfoSearchResult> GetUserInfo(string userId, params KeyValuePair<string, object>[] attributesOrFilters)
        {
            throw new NotImplementedException();
        }

        public Task<MLUserInfoSearchResult> GetUserProfile()
        {
            throw new NotImplementedException();
        }

        public Task<MLAccountBalance> GetUserAccountBalance()
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserAddress>> GetUserAddress(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserInfo(object customData)
        {
            throw new NotImplementedException();
        }

        public Task<Item> GetItemDetails(string itemId, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<MLCategorySearchResult> GetCategoryDetail(string categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<MLSearchResult> ListProductsByCategory(string categoryId, int pageIndex = 0, int pageSize = 0)
        {
            throw new NotImplementedException();
        }

        public Task<MLSearchResult> ListProductsByName(string productName, int pageIndex = 0, int pageSize = 0)
        {
            throw new NotImplementedException();
        }

        public Task<ProductQuestionContent> AskQuestion(string question, string productId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PaymentMethod>> ListPaymentMethods(string paisId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PaymentMethod>> ListUserPaymentMethods()
        {
            throw new NotImplementedException();
        }

        public Task<ProductQuestion> ListQuestionsByProduct(string itemId, int pageIndex = 0, int pageSize = 0,
            params KeyValuePair<string, object>[] attributesAndFilters)
        {
            throw new NotImplementedException();
        }

        public Task<MLOrder> ListOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<MLOrder> ListMyOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<MLOrderInfo> GetOrderDetail(string orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RefreshAuthentication()
        {
            throw new NotImplementedException();
        }

        public Task<Shipping> GetShippingDetails(string shipId)
        {
            throw new NotImplementedException();
        }

        public Task<Feedback> GetOrderFeedback(string orderId)
        {
            throw new NotImplementedException();
        }

        public Task<MLProductDescription> GetProductDescrition(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<MLMyItemsSearchResult> ListMyItems(string status, int pageIndex = 0, int pageSize = 0)
        {
            throw new NotImplementedException();
        }

        public Task<MLMyItemsSearchResult> ListMyItems(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MLListType>> ListTypes(string countryId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeProductStatus(string productId, MLProductStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveProduct(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RelistProduct(string productId, int quantity, double price, string listTypeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeProductAttributes(string productId, dynamic attributes)
        {
            throw new NotImplementedException();
        }

        public Task<ShippingCost> GetShippingCost(string productId, string zipCode)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MLBookmarkItem>> GetBookmarkItems()
        {
            throw new NotImplementedException();
        }

        public Task<bool> BookmarkItem(string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveBookmarkItem(string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<MLAutorizationInfo> TryRefreshToken()
        {
            throw new NotImplementedException();
        }

        public Task<MLProductVisits> GetProductVisits(string productId, DateTime startDate, DateTime finishDate)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MLListPrice>> GetListingPrices(string countryId, double productPrice, params KeyValuePair<string, object>[] attributesOrFilters)
        {
            throw new NotImplementedException();
        }

        public Task<Item> ListNewItem(SellItem itemInfo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RevokeAccess()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MLListType>> GetAvailableUpgrades(string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeItemListType(string itemId, string id_type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendSellerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message,
            MLSellerRatingReason reason, bool restockItem = false, bool hasSellerRefundMoney = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendBuyerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message, MLBuyerRatingReason reason)
        {
            throw new NotImplementedException();
        }

        public Task<MLOrder> ListRecentOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<MLImage> UploadProductImage(ProductImage image)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddPicture(string pictureId, string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<MLErrorRequest> ValidateNewItem(SellItem itemInfo)
        {
            throw new NotImplementedException();
        }

        public Task<MLOrder> ListArchivedSellerOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<MLOrder> ListArchivedBuyerOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeProductDescription(string productId, string text)
        {
            throw new NotImplementedException();
        }

        public Task<MLSearchResult> ListProductsByUser(string userId, int pageIndex = 0, int pageSize = 0)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Country>> ListCountries()
        {
            throw new NotImplementedException();
        }
    }
}
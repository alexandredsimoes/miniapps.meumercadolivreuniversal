using GalaSoft.MvvmLight;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{

    public enum MLRating
    {
        positive,
        negative,
        neutral
    }

    public enum MLSellerRatingReason
    {
        SELLER_REGRETS,
        THEY_DIDNT_ANSWER,
        BUYER_REGRETS,
        SELLER_OUT_OF_STOCK,
        SELLER_DIDNT_TRY_TO_CONTACT_BUYER,
        BUYER_NOT_ENOUGH_MONEY,
        THEY_NOT_HONORING_POLICIES,
        OTHER_MY_RESPONSIBILITY,
        OTHER_THEIR_RESPONSIBILITY
    }

    public enum MLBuyerRatingReason
    {
        SELLER_OUT_OF_STOCK,
        BUYER_PAID_BUT_DID_NOT_RECEIVE,
        OTHER_MY_RESPONSIBILITY        
    }

    public enum MLProductStatus
    {
        mlpsActive,
        mlpsPause,
        mlpsClose
    }

    public class VisitsDetail
    {
        public string company { get; set; }
        public int quantity { get; set; }
    }

    public class MLProductVisits
    {
        public string item_id { get; set; }
        public string date_from { get; set; }
        public string date_to { get; set; }
        public int total_visits { get; set; }
        public List<VisitsDetail> visits_detail { get; set; }
    }

    public class PaymentMethod
    {
        public string id { get; set; }
        public string name { get; set; }
        public string payment_type_id { get; set; }
        public string thumbnail { get; set; }
        public string secure_thumbnail { get; set; }
    }

    public class Picture
    {
        public string id { get; set; }
        public string url { get; set; }
        public string secure_url { get; set; }
        public string size { get; set; }
        public string max_size { get; set; }
        public string quality { get; set; }
    }

    public class Paging
    {
        public int total { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
    }

    public class Seller
    {
        public long? id { get; set; }
        public string power_seller_status { get; set; }
        public bool? car_dealer { get; set; }
        public bool? real_estate_agency { get; set; }
        public string nickname { get; set; }
        public string email { get; set; }
        public Phone2 phone { get; set; }
        public AlternativePhone2 alternative_phone { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class Installments
    {
        public int? quantity { get; set; }
        public double? amount { get; set; }
        public string currency_id { get; set; }
    }

    public class Address
    {
        public string state_id { get; set; }
        public string state_name { get; set; }
        public string city_id { get; set; }
        public string city_name { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string zip_code { get; set; }
    }

    public class Phone
    {
        public string area_code { get; set; }
        public string number { get; set; }
        public object extension { get; set; }
        public bool verified { get; set; }
    }

    public class AlternativePhone
    {
        public string area_code { get; set; }
        public string number { get; set; }
        public object extension { get; set; }
    }

    public class Country
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class State
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class City
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class SellerAddress
    {
        public long? id { get; set; }
        public string comment { get; set; }
        public string address_line { get; set; }
        public string zip_code { get; set; }
        public Country country { get; set; }
        public State state { get; set; }
        public City city { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
    }

    public class DifferentialPricing
    {
        public long? id { get; set; }
        public List<string> payment_methods { get; set; }
        public List<int> installments { get; set; }
    }

    public class Item
    {
        public Item()
        {
            IsFavorite = false;
        }
        [PrimaryKey]
        public int ItemId { get; set; }
        public string id { get; set; }
        public double? seller_id { get; set; }
        public string site_id { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        [Ignore]
        public Seller seller { get; set; }
        public double? price { get; set; }
        public string currency_id { get; set; }
        public int? available_quantity { get; set; }
        public int? sold_quantity { get; set; }
        public string buying_mode { get; set; }
        public string listing_type_id { get; set; }
        public string stop_time { get; set; }
        public string condition { get; set; }
        public string permalink { get; set; }
        public string thumbnail { get; set; }
        public bool? accepts_mercadopago { get; set; }
        [Ignore]
        public Installments installments { get; set; }
        [Ignore]
        public Address address { get; set; }
        [Ignore]
        public Shipping shipping { get; set; }
        [Ignore]
        public SellerAddress seller_address { get; set; }
        [Ignore]
        public List<object> attributes { get; set; }
        [Ignore]
        public List<Picture> pictures { get; set; }

        public double? original_price { get; set; }
        public string category_id { get; set; }
        [Ignore]
        public DifferentialPricing differential_pricing { get; set; }
        [Ignore]
        public List<object> variation_attributes { get; set; }
        public string variation_id { get; set; }
        [Ignore]
        public MLListType ListType { get; set; }
        public bool IsFavorite { get; set; }
        public string status { get; set; }

        public int CompareTo(Item other)
        {
            return this.title.CompareTo(other.title);
        }
    }

    public class Sort
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class AvailableSort
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class PathFromRoot
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<PathFromRoot> path_from_root { get; set; }
    }

    public class Filter
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<Value> values { get; set; }
    }

    public class Value2
    {
        public AvailableFilter filter { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public long? results { get; set; }
        public bool IsSelected { get; set; } = false;
    }

    public class AvailableFilter
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public ObservableCollection<Value2> values { get; set; } = new ObservableCollection<Value2>();        

        public AvailableFilter()
        {
            values.CollectionChanged += Values_CollectionChanged;
        }

        private void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                (((ObservableCollection<Value2>)sender)[e.NewStartingIndex]).filter = this;
        }
    }

    public class MLBookmarkItem
    {
        public string bookmarked_date { get; set; }
        public string item_id { get; set; }
        public Item ItemInfo { get; set; }
    }

    public class MLSearchResult
    {
        public string site_id { get; set; }
        public Paging paging { get; set; }
        public List<Item> results { get; set; }
        public List<object> secondary_results { get; set; }
        public List<object> related_results { get; set; }
        public Sort sort { get; set; }
        public List<AvailableSort> available_sorts { get; set; }
        public List<Filter> filters { get; set; }
        public List<AvailableFilter> available_filters { get; set; }
    }
}

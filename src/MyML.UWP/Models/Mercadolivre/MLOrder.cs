using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    //public class Paging
    //{
    //    public int total { get; set; }
    //    public int offset { get; set; }
    //    public int limit { get; set; }
    //}

    public class StatusDetail
    {
        public object description { get; set; }
        public string code { get; set; }
    }

    //public class Item
    //{
    //    public string id { get; set; }
    //    public string title { get; set; }
    //    public List<object> variation_attributes { get; set; }
    //    public string category_id { get; set; }
    //    public object variation_id { get; set; }
    //}

    public class OrderItem
    {
        public string currency_id { get; set; }
        public Item item { get; set; }
        public double? sale_fee { get; set; }
        public double? quantity { get; set; }
        public double? unit_price { get; set; }
    }

    public class Collector
    {
        public long? id { get; set; }
    }

    public class AtmTransferReference
    {
        public object company_id { get; set; }
        public object transaction_id { get; set; }
    }

    public class Payment
    {
        public long? id { get; set; }
        public long? order_id { get; set; }
        public long? payer_id { get; set; }
        public Collector collector { get; set; }
        public string currency_id { get; set; }
        public string status { get; set; }
        public string status_code { get; set; }
        public string status_detail { get; set; }
        public double? transaction_amount { get; set; }
        public double? shipping_cost { get; set; }
        public double? overpaid_amount { get; set; }
        public double? total_paid_amount { get; set; }
        public double? marketplace_fee { get; set; }
        public double? coupon_amount { get; set; }
        public string date_created { get; set; }
        public string date_last_modified { get; set; }
        public long? card_id { get; set; }
        public string reason { get; set; }
        public object activation_uri { get; set; }
        public string payment_method_id { get; set; }
        public int? installments { get; set; }
        public string issuer_id { get; set; }
        public AtmTransferReference atm_transfer_reference { get; set; }
        public object coupon_id { get; set; }
        public string operation_type { get; set; }
        public string payment_type { get; set; }
        public List<string> available_actions { get; set; }
    }

    public class EstimatedDelivery
    {
        public DateTimeOffset? date { get; set; }
        public object time_from { get; set; }
        public object time_to { get; set; }
    }

    public class BillingInfo
    {
        public string doc_type { get; set; }
        public string doc_number { get; set; }
    }

    public class Buyer
    {
        public long? id { get; set; }
        public string nickname { get; set; }
        public string email { get; set; }
        public Phone phone { get; set; }
        public AlternativePhone alternative_phone { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public BillingInfo billing_info { get; set; }
    }

    public class Phone2
    {
        public string area_code { get; set; }
        public string number { get; set; }
        public object extension { get; set; }
    }

    public class AlternativePhone2
    {
        public string area_code { get; set; }
        public string number { get; set; }
        public object extension { get; set; }
    }

    //public class Seller
    //{
    //    public int id { get; set; }
    //    public string nickname { get; set; }
    //    public string email { get; set; }
    //    public Phone2 phone { get; set; }
    //    public AlternativePhone2 alternative_phone { get; set; }
    //    public string first_name { get; set; }
    //    public string last_name { get; set; }
    //}

    public class Sale
    {
        public To to { get; set; }
        public object has_seller_refunded_money { get; set; }
        public string status { get; set; }
        public object reason { get; set; }
        public string site_id { get; set; }
        public string date_created { get; set; }
        public FeedbackFrom from { get; set; }
        public long? order_id { get; set; }
        public bool? modified { get; set; }
        public long id { get; set; }
        public string message { get; set; }
        public bool? fulfilled { get; set; }
        public Item item { get; set; }
        public string visibility_date { get; set; }
        public object reply { get; set; }
        public string role { get; set; }
        public string app_id { get; set; }
        public string rating { get; set; }
        public bool? restock_item { get; set; }
    }

    public class Purchase
    {
        public To2 to { get; set; }
        public object has_seller_refunded_money { get; set; }
        public string status { get; set; }
        public object reason { get; set; }
        public string site_id { get; set; }
        public string date_created { get; set; }
        public From2 from { get; set; }
        public long? order_id { get; set; }
        public bool? modified { get; set; }
        public long? id { get; set; }
        public string message { get; set; }
        public bool? fulfilled { get; set; }
        public Item2 item { get; set; }
        public string visibility_date { get; set; }
        public object reply { get; set; }
        public string role { get; set; }
        public string app_id { get; set; }
        public string rating { get; set; }
    }

    //public class Feedback
    //{
    //    public Sale sale { get; set; }
    //    public Purchase purchase { get; set; }
    //}

    public class MLOrderInfo
    {
        public long? id { get; set; }
        public object comments { get; set; }
        public string status { get; set; }
        public StatusDetail status_detail { get; set; }
        public string date_created { get; set; }
        public string date_closed { get; set; }
        public string date_last_updated { get; set; }
        public bool? hidden_for_seller { get; set; }
        public string currency_id { get; set; }
        public List<OrderItem> order_items { get; set; }
        public double? total_amount { get; set; }
        public double? total_amount_with_shipping { get; set; }
        public List<object> mediations { get; set; }
        public List<Payment> payments { get; set; }
        public Shipping shipping { get; set; }
        public Buyer buyer { get; set; }
        public Seller seller { get; set; }
        public Feedback feedback { get; set; }
        public List<string> tags { get; set; }
    }

    //public class Sort
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //}

    //public class AvailableSort
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //}


    //public class AvailableFilter
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string type { get; set; }
    //    public List<Value> values { get; set; }
    //}

    public class MLOrder
    {
        public string query { get; set; }
        public string display { get; set; }
        public Paging paging { get; set; }
        public List<MLOrderInfo> results { get; set; }
        public Sort sort { get; set; }
        public List<AvailableSort> available_sorts { get; set; }
        public List<object> filters { get; set; }
        public List<AvailableFilter> available_filters { get; set; }
    }
}

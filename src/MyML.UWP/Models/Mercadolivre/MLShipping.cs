using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class StatusHistory
    {
        public string date_shipped { get; set; }
        public string date_delivered { get; set; }
    }

    //public class City
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //}

    //public class State
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //}

    //public class Country
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //}

    public class Neighborhood
    {
        public object id { get; set; }
        public string name { get; set; }
    }

    public class Municipality
    {
        public object id { get; set; }
        public object name { get; set; }
    }

    public class SenderAddress
    {
        public long? id { get; set; }
        public string address_line { get; set; }
        public string street_name { get; set; }
        public string street_number { get; set; }
        public object comment { get; set; }
        public string zip_code { get; set; }
        public City city { get; set; }
        public State state { get; set; }
        public Country country { get; set; }
        public Neighborhood neighborhood { get; set; }
        public Municipality municipality { get; set; }
        public List<string> types { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public object geolocation_type { get; set; }
        public object agency { get; set; }
        public object is_valid_for_carrier { get; set; }
    }

    public class City2
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class State2
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Country2
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Neighborhood2
    {
        public object id { get; set; }
        public string name { get; set; }
    }

    public class Municipality2
    {
        public object id { get; set; }
        public object name { get; set; }
    }

    public class ReceiverAddress
    {
        public long? id { get; set; }
        public string address_line { get; set; }
        public string street_name { get; set; }
        public string street_number { get; set; }
        public string comment { get; set; }
        public string zip_code { get; set; }
        public City2 city { get; set; }
        public State2 state { get; set; }
        public Country2 country { get; set; }
        public Neighborhood2 neighborhood { get; set; }
        public Municipality2 municipality { get; set; }
        public object types { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public object geolocation_type { get; set; }
        public object agency { get; set; }
        public object is_valid_for_carrier { get; set; }
        public object receiver_name { get; set; }
        public object receiver_phone { get; set; }
    }

    public class ShippingItem
    {
        public string id { get; set; }
        public string description { get; set; }
        public double? quantity { get; set; }
        public object dimensions { get; set; }
    }

    public class Speed
    {
        public long? shipping { get; set; }
        public object handling { get; set; }
    }

    public class ShippingOption
    {
        public object id { get; set; }
        public object shipping_method_id { get; set; }
        public string name { get; set; }
        public string currency_id { get; set; }
        public double? list_cost { get; set; }
        public double? cost { get; set; }
        public Speed speed { get; set; }
        public EstimatedDelivery estimated_delivery { get; set; }
    }

    public class CostComponents
    {
        public double? special_discount { get; set; }
    }

    public class Shipping
    {
        public long? id { get; set; }
        public bool free_shipping { get; set; }
        public string mode { get; set; }
        public string created_by { get; set; }
        public long? order_id { get; set; }
        public object order_cost { get; set; }
        public string site_id { get; set; }
        public string status { get; set; }
        public object substatus { get; set; }
        public StatusHistory status_history { get; set; }
        public string date_created { get; set; }
        public string last_updated { get; set; }
        public string tracking_number { get; set; }
        public string tracking_method { get; set; }
        public object service_id { get; set; }
        public double? sender_id { get; set; }
        public SenderAddress sender_address { get; set; }
        public double? receiver_id { get; set; }
        public ReceiverAddress receiver_address { get; set; }
        public List<ShippingItem> shipping_items { get; set; }
        public ShippingOption shipping_option { get; set; }
        public string comments { get; set; }
        public object date_first_printed { get; set; }
        public string market_place { get; set; }
        public object return_details { get; set; }
        public object return_tracking_number { get; set; }
        public object carrier_id { get; set; }
        public CostComponents cost_components { get; set; }
        public string currency_id { get; set; }
        public object shipping_mode { get; set; }
        public string shipment_type { get; set; }        
        public object picking_type { get; set; }              
        public double? cost { get; set; }
        public List<FreeMethod> free_methods { get; set; }

    }

    public class Rule
    {
        public string free_mode { get; set; }
        public List<string> value { get; set; }
    }

    public class FreeMethod
    {
        public int? id { get; set; }
        public Rule rule { get; set; }
    }

    public class ZipCodeType
    {
        public string type { get; set; }
        public string description { get; set; }
    }

    public class ExtendedAttributes
    {
        public string address { get; set; }
        public object owner_name { get; set; }
        public ZipCodeType zip_code_type { get; set; }
        public string city_type { get; set; }
        public string city_name { get; set; }
        public string neighborhood { get; set; }
        public string status { get; set; }
    }

    public class Destination
    {
        public string zip_code { get; set; }
        public City city { get; set; }
        public State state { get; set; }
        public Country country { get; set; }
        public ExtendedAttributes extended_attributes { get; set; }
    }

    public class Discount
    {
        public double? rate { get; set; }
        public string type { get; set; }
        public double? promoted_amount { get; set; }
    }

    public class Option
    {
        public string id { get; set; }
        public string name { get; set; }
        public string shipping_method_id { get; set; }
        public string currency_id { get; set; }
        public double? list_cost { get; set; }
        public double? cost { get; set; }
        public string tracks_shipments_status { get; set; }
        public string display { get; set; }
        public Speed speed { get; set; }
        public EstimatedDelivery estimated_delivery { get; set; }
        public Discount discount { get; set; }
    }

    public class ShippingCost
    {
        public Destination destination { get; set; }
        public List<Option> options { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class Identification
    {
        public string type { get; set; }
        public string number { get; set; }
    }

    //public class Address
    //{
    //    public string state { get; set; }
    //    public string city { get; set; }
    //    public string address { get; set; }
    //    public string zip_code { get; set; }
    //}


    public class Ratings
    {
        public double? positive { get; set; }
        public double? negative { get; set; }
        public double? neutral { get; set; }
    }

    public class Transactions
    {
        public string period { get; set; }
        public int? total { get; set; }
        public int? completed { get; set; }
        public int? canceled { get; set; }
        public Ratings ratings { get; set; }
    }

    public class SellerReputation
    {
        public object level_id { get; set; }
        public object power_seller_status { get; set; }
        public Transactions transactions { get; set; }
    }

    public class Canceled
    {
        public long? total { get; set; }
        public long? paid { get; set; }
    }

    public class Unrated
    {
        public int? total { get; set; }
        public int? paid { get; set; }
    }

    public class NotYetRated
    {
        public int? total { get; set; }
        public int? paid { get; set; }
        public int? units { get; set; }
    }

    public class Transactions2
    {
        public string period { get; set; }
        public double? total { get; set; }
        public double? completed { get; set; }
        public Canceled canceled { get; set; }
        public Unrated unrated { get; set; }
        public NotYetRated not_yet_rated { get; set; }
    }

    public class BuyerReputation
    {
        public long? canceled_transactions { get; set; }
        public Transactions2 transactions { get; set; }
        public List<object> tags { get; set; }
    }

    public class ImmediatePayment
    {
        public bool? required { get; set; }
        public List<object> reasons { get; set; }
    }

    public class List
    {
        public bool? allow { get; set; }
        public List<object> codes { get; set; }
        public ImmediatePayment immediate_payment { get; set; }
    }

    public class ImmediatePayment2
    {
        public bool required { get; set; }
        public List<object> reasons { get; set; }
    }

    public class Buy
    {
        public bool? allow { get; set; }
        public List<object> codes { get; set; }
        public ImmediatePayment2 immediate_payment { get; set; }
    }

    public class ImmediatePayment3
    {
        public bool? required { get; set; }
        public List<object> reasons { get; set; }
    }

    public class Sell
    {
        public bool? allow { get; set; }
        public List<object> codes { get; set; }
        public ImmediatePayment3 immediate_payment { get; set; }
    }

    public class Billing
    {
        public bool? allow { get; set; }
        public List<object> codes { get; set; }
    }

    public class Status
    {
        public string site_status { get; set; }
        public List list { get; set; }
        public Buy buy { get; set; }
        public Sell sell { get; set; }
        public Billing billing { get; set; }
        public bool? mercadopago_tc_accepted { get; set; }
        public string mercadopago_account_type { get; set; }
        public string mercadoenvios { get; set; }
        public bool? immediate_payment { get; set; }
        public bool? confirmed_email { get; set; }
        public string user_type { get; set; }
        public object required_action { get; set; }
    }

    public class Credit
    {
        public double? consumed { get; set; }
        public string credit_level_id { get; set; }
    }

    public class MLUserInfoSearchResult
    {
        public long? id { get; set; }
        public string nickname { get; set; }
        public DateTimeOffset registration_date { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string country_id { get; set; }
        public string email { get; set; }
        public Identification identification { get; set; }
        public Address address { get; set; }
        public Phone phone { get; set; }
        public AlternativePhone alternative_phone { get; set; }
        public string user_type { get; set; }
        public List<string> tags { get; set; }
        public object logo { get; set; }
        public long? points { get; set; }
        public string site_id { get; set; }
        public string permalink { get; set; }
        public List<string> shipping_modes { get; set; }
        public string seller_experience { get; set; }
        public SellerReputation seller_reputation { get; set; }
        public BuyerReputation buyer_reputation { get; set; }
        public Status status { get; set; }
        public Credit credit { get; set; }
    }
}

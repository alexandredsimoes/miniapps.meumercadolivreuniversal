using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{

    public class SearchLocation
    {
        public State2 state { get; set; }
        public City2 city { get; set; }
        public Neighborhood2 neighborhood { get; set; }
    }

    public class OnHolidays
    {
        public List<object> hours { get; set; }
        public string status { get; set; }
    }

    public class OpenHours
    {
        public OnHolidays on_holidays { get; set; }
    }

    public class UserAddress
    {
        public long? id { get; set; }
        public long? user_id { get; set; }
        public string contact { get; set; }
        public string phone { get; set; }
        public string address_line { get; set; }
        public object floor { get; set; }
        public object apartment { get; set; }
        public string street_number { get; set; }
        public string street_name { get; set; }
        public string zip_code { get; set; }
        public City city { get; set; }
        public State state { get; set; }
        public Country country { get; set; }
        public Neighborhood neighborhood { get; set; }
        public Municipality municipality { get; set; }
        public SearchLocation search_location { get; set; }
        public List<object> types { get; set; }
        public string comment { get; set; }
        public string geolocation_type { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string status { get; set; }
        public string date_created { get; set; }
        public bool? normalized { get; set; }
        public OpenHours open_hours { get; set; }
    }
}

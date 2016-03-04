using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    
    //public class PathFromRoot
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //}

    public class ChildrenCategory
    {
        public string id { get; set; }
        public string name { get; set; }
        public long? total_items_in_this_category { get; set; }
    }

    public class Settings
    {
        public bool adult_content { get; set; }
        public bool buying_allowed { get; set; }
        public List<string> buying_modes { get; set; }
        public string coverage_areas { get; set; }
        public List<string> currencies { get; set; }
        public bool fragile { get; set; }
        public string immediate_payment { get; set; }
        public List<string> item_conditions { get; set; }
        public bool items_reviews_allowed { get; set; }
        public int? max_description_length { get; set; }
        public int? max_pictures_per_item { get; set; }
        public int? max_sub_title_length { get; set; }
        public int? max_title_length { get; set; }
        public string price { get; set; }
        public List<object> restrictions { get; set; }
        public bool rounded_address { get; set; }
        public string seller_contact { get; set; }
        public List<string> shipping_modes { get; set; }
        public List<string> shipping_options { get; set; }
        public string shipping_profile { get; set; }
        public bool show_contact_information { get; set; }
        public string simple_shipping { get; set; }
        public string stock { get; set; }
        public List<object> tags { get; set; }
        public string vip_subdomain { get; set; }
        public object mirror_category { get; set; }
        public bool listing_allowed { get; set; }
        public object maximum_price { get; set; }
        public object minimum_price { get; set; }
    }

    public class MLCategorySearchResult
    {
        public string id { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string permalink { get; set; }
        public long? total_items_in_this_category { get; set; }
        public List<PathFromRoot> path_from_root { get; set; }
        public List<ChildrenCategory> children_categories { get; set; }
        public string attribute_types { get; set; }
        public Settings settings { get; set; }
    }
}

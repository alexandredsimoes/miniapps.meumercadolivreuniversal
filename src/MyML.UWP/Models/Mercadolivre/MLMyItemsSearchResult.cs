using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{

    public class Order
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class AvailableOrder
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    //public class AvailableFilterMyItems
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public List<Value2> values { get; set; }
    //}

    

    public class MLMyItemsSearchResult
    {
        public string seller_id { get; set; }
        public object query { get; set; }
        public Paging paging { get; set; }
        public List<string> results { get; set; }
        public List<Item> results_graph { get; set; }        
        public List<Order> orders { get; set; }
        public List<object> filters { get; set; }
        public List<AvailableOrder> available_orders { get; set; }
        public List<AvailableFilter> available_filters { get; set; }
        public IList<MLListType> ListTypes { get; set; }
    }
}

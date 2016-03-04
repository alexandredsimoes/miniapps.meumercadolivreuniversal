using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class To
    {
        public long? id { get; set; }
        public string status { get; set; }
        public string nickname { get; set; }
        public int? points { get; set; }
    }

    public class FeedbackFrom
    {
        public long? id { get; set; }
        public string status { get; set; }
        public string nickname { get; set; }
        public int? points { get; set; }
    }

    public class To2
    {
        public long? id { get; set; }
        public string status { get; set; }
        public string nickname { get; set; }
        public int? points { get; set; }
    }

    public class From2
    {
        public long? id { get; set; }
        public string status { get; set; }
        public string nickname { get; set; }
        public int? points { get; set; }
    }

    public class Item2
    {
        public string id { get; set; }
        public string title { get; set; }
        public double? price { get; set; }
        public string currency_id { get; set; }
    }



    public class Feedback
    {
        public Sale sale { get; set; }
        public Purchase purchase { get; set; }
    }
}

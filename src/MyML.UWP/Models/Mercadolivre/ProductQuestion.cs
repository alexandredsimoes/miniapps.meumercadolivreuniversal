﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class Answer
    {
        
        public int AnswerId { get; set; }
        public string text { get; set; }
        public string status { get; set; }
        public string date_created { get; set; }
    }

    public class ProductQuestionContent
    {
        
        public int ProductQuestionContentId { get; set; }
        public DateTime? date_created { get; set; }
        public string item_id { get; set; }
        public long? seller_id { get; set; }
        public string status { get; set; }
        public string text { get; set; }
        public long? id { get; set; }
        
        public Answer answer { get; set; }

        
        public int AnswerId { get; set; }

        
        public int ItemId { get; set; }

        
        public Item Item { get; set; }
        public string nickname { get; set; }
        public string buyer_experience { get; set; }
        public DateTimeOffset? registration_date { get; set; }
        public long? buyer_id { get; set; }
    }

    public class FiltersQuestion
    {
        public int? limit { get; set; }
        public int? offset { get; set; }
        public bool? is_admin { get; set; }
        public List<object> sorts { get; set; }
        public object caller { get; set; }
        public string item { get; set; }
    }

    public class AvailableFilters
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<string> values { get; set; }
    }

    public class ProductQuestion
    {
        public int? total { get; set; }
        public int? limit { get; set; }
        public List<ProductQuestionContent> questions { get; set; }
        public FiltersQuestion filters { get; set; }
        public IList<AvailableFilters> available_filters { get; set; }
        public IList<string> available_sorts { get; set; }
    }
}

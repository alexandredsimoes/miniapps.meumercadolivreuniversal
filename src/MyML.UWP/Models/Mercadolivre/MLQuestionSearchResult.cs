using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class From
    {
        public long? id { get; set; }
        public int answered_questions { get; set; }
        public MLUserInfoSearchResult UserInfo { get; set; }
    }

    public class Question
    {
        public DateTime? date_created { get; set; }
        public string item_id { get; set; }
        public long? seller_id { get; set; }
        public string status { get; set; }
        public string text { get; set; }

        public long? id { get; set; }
        public bool? deleted_from_listing { get; set; }
        public bool? hold { get; set; }
        public Answer answer { get; set; }
        public From From { get; set; }

        //Dados do produto
        public Item ProductInfo { get; set; }
    }

    public class Filters
    {
        public int? limit { get; set; }
        public int? offset { get; set; }
        public bool? is_admin { get; set; }
        public List<object> sorts { get; set; }
        public long? caller { get; set; }
        public string seller { get; set; }
        public string status { get; set; }
    }

    public class AvailableFilterQuestion
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<string> values { get; set; }
    }

    public class MLQuestionResultSearch
    {
        public long? total { get; set; }
        public int? limit { get; set; }
        public List<Question> questions { get; set; }
        public Filters filters { get; set; }
        public List<AvailableFilterQuestion> available_filters { get; set; }
        public List<string> available_sorts { get; set; }
    }
}

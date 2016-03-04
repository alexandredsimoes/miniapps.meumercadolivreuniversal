using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyML.BackendServices.Models
{
    public class ML
    {
        public string _id { get; set; }
        public string resource { get; set; }
        public object user_id { get; set; }
        public string topic { get; set; }
        public object application_id { get; set; }
        public int attempts { get; set; }
        public string sent { get; set; }
        public string received { get; set; }
        public MLRequest request { get; set; }
        public MLResponse response { get; set; }
    }

    public class MLResponse
    {
        public int http_code { get; set; }
        public string body { get; set; }
        public int req_time { get; set; }
        //public Headers2 headers { get; set; }
    }

    public class MLRequest
    {
        public string url { get; set; }
        //public Headers headers { get; set; }
        public string data { get; set; }


        public class MLAnswer
        {
            public string date_created { get; set; }
            public string status { get; set; }
            public string text { get; set; }
        }

        public class MLQuestion
        {
            public long id { get; set; }
            public MLAnswer answer { get; set; }
            public string date_created { get; set; }
            public string item_id { get; set; }
            public int seller_id { get; set; }
            public string status { get; set; }
            public string text { get; set; }
        }
    }


    public class Answer
    {
        public string date_created { get; set; }
        public string status { get; set; }
        public string text { get; set; }
    }

    public class QuestionInfo
    {
        public long? id { get; set; }
        public Answer answer { get; set; }
        public string date_created { get; set; }
        public string item_id { get; set; }
        public long? seller_id { get; set; }
        public string status { get; set; }
        public string text { get; set; }
    }
}

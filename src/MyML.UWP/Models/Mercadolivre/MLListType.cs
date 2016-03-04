using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class MLListType
    {
        public string site_id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public MLListPrice Price { get; set; }
    }   
}

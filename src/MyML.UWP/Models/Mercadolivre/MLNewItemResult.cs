using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Models.Mercadolivre
{
    public class MLNewItemResult
    {
        public string id { get; set; }
        public string site_id { get; set; }
        public string title { get; set; }
        public int? sold_quantity { get; set; }
        public string permalink { get; set; }
    }
}

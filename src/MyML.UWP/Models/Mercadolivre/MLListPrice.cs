using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Models.Mercadolivre
{
    public class MLListPrice
    {
        public string listing_type_id { get; set; }
        public string listing_type_name { get; set; }
        public string listing_exposure { get; set; }
        public bool? requires_picture { get; set; }
        public string currency_id { get; set; }
        public double? listing_fee_amount { get; set; }
        public double? sale_fee_amount { get; set; }
        public bool? free_relist { get; set; }
        public string stop_time { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class MLProductDescription
    {
        public string text { get; set; }
        public string plain_text { get; set; }
        public DateTime? last_updated { get; set; }
        public DateTime? date_created { get; set; }        
    }
}

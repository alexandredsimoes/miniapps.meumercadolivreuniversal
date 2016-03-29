using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Models.Mercadolivre
{
    public class Variation
    {
        public string size { get; set; }
        public string url { get; set; }
        public string secure_url { get; set; }
    }

    public class MLImage
    {
        public string id { get; set; }
        public string max_size { get; set; }
        public List<Variation> variations { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class Cause
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class MLErrorRequest
    {
        public string message { get; set; }
        public string error { get; set; }
        public long? status { get; set; }
        public List<Cause> cause { get; set; }
    }
}

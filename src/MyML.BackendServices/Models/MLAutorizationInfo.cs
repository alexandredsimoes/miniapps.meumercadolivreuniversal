using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyML.BackendServices.Models
{
    public class MLAutorizationInfo
    {
        public string user_id { get; set; }
        public string Token_Type { get; set; }
        public double? Expires_In { get; set; }
        public string Scope { get; set; }
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
    }
}

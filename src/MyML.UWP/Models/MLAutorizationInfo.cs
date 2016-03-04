using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models
{
    public class MLAutorizationInfo
    {
        public int MLAutorizationInfoId { get; set; }
        public string user_id { get; set; }
        public string Token_Type { get; set; }
        public double? Expires_In { get; set; }
        public string Scope { get; set; }
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
    }
}

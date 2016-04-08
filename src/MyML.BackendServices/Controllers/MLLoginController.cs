using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyML.Web.Admin.Controllers
{
    [Authorize]
    public class MLLoginController : Controller
    {
        // GET: MLLogin
        public ActionResult Index(string code = null)
        {
            if(code == null)
            {
                var clientId = "8765232316929095";
                var url = String.Format("https://auth.mercadolivre.com.br/authorization?response_type=code&client_id={0}&redirect_uri={1}",
                    clientId, "");
                return Redirect(url);
            }
            else
            {
                var config = ConfigurationManager.AppSettings[""] = "";
            }

            
            return View();
        }
    }
}
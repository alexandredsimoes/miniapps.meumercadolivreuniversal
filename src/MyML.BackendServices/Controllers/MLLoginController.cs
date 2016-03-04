using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MyML.BackendServices.Controllers
{
    [Authorize]
    public class MLLoginController : Controller
    {
        // GET: MLLogin
        public ActionResult Index(string code = null)
        {
            if (code == null)
            {
                var clientId = "8765232316929095";
                var url = String.Format("https://auth.mercadolivre.com.br/authorization?response_type=code&client_id={0}&redirect_uri={1}",
                    clientId, "");
                return Redirect(url);
            }
            else
            {
                var config = Startup.Configuration;
            }


            return View();
        }
    }
}

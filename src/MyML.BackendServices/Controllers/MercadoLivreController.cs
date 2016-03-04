using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using MyML.BackendServices.Models;
using Microsoft.AspNet.Http.Internal;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MyML.BackendServices.Controllers
{
    public class MercadoLivreController : Controller
    {
        private string ML_API_KEY = "1pW3zr833brws5ePUbtfcQc52xzq1Ocq";
        private string ML_RETURN_URL = "https://meumercadolivre.azurewebsites.net/mercadolivre/login";
        private string ML_CLIENT_ID = "8765232316929095";
        private string ML_AUTORIZATION_URL = "https://api.mercadolibre.com/oauth/token";
        // GET: MercadoLivre
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Login(string code = null)
        {
            if (code == null)
            {

                var url = String.Format("https://auth.mercadolivre.com.br/authorization?response_type=code&client_id={0}&redirect_uri={1}",
                    ML_CLIENT_ID, "");

                return Redirect(url);
                return View();
            }
            else
            {
                List<KeyValuePair<string, string>> parametros = new List<KeyValuePair<string, string>>();

                parametros.Add(new KeyValuePair<string, string>("client_id", ML_CLIENT_ID));
                parametros.Add(new KeyValuePair<string, string>("client_secret", ML_API_KEY));
                parametros.Add(new KeyValuePair<string, string>("redirect_uri", ML_RETURN_URL));
                parametros.Add(new KeyValuePair<string, string>("code", code));
                parametros.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

#if DEBUG
                Debug.WriteLine("Enviando codigo recebido " + code);
#endif
                //var autorizationUrl = Consts.GetUrl(Consts.ML_AUTORIZATION_URL);
                HttpClient httpClient = new HttpClient();
                var result = await httpClient.PostAsync(ML_AUTORIZATION_URL, new FormUrlEncodedContent(parametros));
                var content = await result.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<MLAutorizationInfo>(content);


                var webConfig = Startup.Configuration;
                webConfig["ml:token"] = userInfo.Access_Token;
                webConfig["ml:refreshToken"] = userInfo.Refresh_Token;
                webConfig["ml:userId"] = userInfo.Refresh_Token;
                webConfig["ml:keyExpires"] = DateTime.Now.AddSeconds(userInfo.Expires_In ?? 0).ToString();
                webConfig["ml:loginDate"] = DateTime.Now.ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult ProcessarNotificacao(FormCollection collection)
        {
            return View();
        }
    }
}

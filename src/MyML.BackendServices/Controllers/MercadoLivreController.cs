using MyML.BackendServices.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MyML.Web.Admin.Controllers
{
    public class MercadoLivreController : Controller
    {
        private string ML_API_KEY = "1pW3zr833brws5ePUbtfcQc52xzq1Ocq";
        private string ML_RETURN_URL = "https://www.miniapps.com.br/api/mercadolivre/login";
        private string ML_CLIENT_ID = "8765232316929095";
        private string ML_AUTORIZATION_URL = "https://api.mercadolibre.com/oauth/token";
        // GET: MercadoLivre
        public ActionResult Index()
        {
            return View();
        }
       
        public async Task<ActionResult> Login(string code = null)
        {
            try
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

                    Configuration webConfig = WebConfigurationManager.OpenWebConfiguration("~");
                    webConfig.AppSettings.Settings["ml:token"].Value = userInfo.Access_Token;
                    webConfig.AppSettings.Settings["ml:refreshToken"].Value = userInfo.Refresh_Token;
                    webConfig.AppSettings.Settings["ml:userId"].Value = userInfo.Refresh_Token;
                    webConfig.AppSettings.Settings["ml:keyExpires"].Value = DateTime.Now.AddSeconds(userInfo.Expires_In ?? 0).ToString();
                    webConfig.AppSettings.Settings["ml:loginDate"].Value = DateTime.Now.ToString();
                    webConfig.Save();

                    return View();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ProcessarNotificacao(FormCollection collection)
        {
            return View();
        }
    }
}
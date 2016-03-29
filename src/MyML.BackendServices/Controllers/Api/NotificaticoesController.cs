using Microsoft.AspNet.Mvc;
using MyML.BackendServices.Models;
using MyML.BackendServices.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace MyML.BackendServices.Controllers.Api
{
    public class NotificacoesController : Controller
    {
        private string ML_QUESTION_DETAIL_URL = "https://api.mercadolibre.com/questions/{0}?access_token={1}";
        private string ML_ACCESS_TOKEN = "";
        private string ML_USER_ID = "";
        private string ML_REFRESH_TOKEN_URL = "https://api.mercadolibre.com/oauth/token?grant_type=refresh_token&client_id={0}&client_secret={1}&refresh_token={2}";
        private string ML_REFRESH_TOKEN = "";
        private string ML_API_KEY = "1pW3zr833brws5ePUbtfcQc52xzq1Ocq";
        private string ML_CLIENT_ID = "8765232316929095";

        HttpClient _client;
        private readonly IDataService _dataService;
        private readonly IMailServices _mailServices;

        public NotificacoesController(IDataService dataService, IMailServices mailServices)
        {
            _mailServices = mailServices;
            _dataService = dataService;
            var config = Startup.Configuration;
            ML_ACCESS_TOKEN = config["ml:token"];
            ML_USER_ID = config["ml:userId"];
            ML_REFRESH_TOKEN = config["ml:refreshToken"];

            ML_REFRESH_TOKEN_URL = string.Format(ML_REFRESH_TOKEN_URL,
                ML_CLIENT_ID, ML_API_KEY, ML_REFRESH_TOKEN);

            _client = new HttpClient();
            _dataService = new DataService();
        }
        public IHttpActionResult Get()
        {
            return Ok();
        }

        public async Task<IHttpActionResult> Post(ML mlObject)
        {
            var config = Startup.Configuration;
            if (!_dataService.EstaAutenticado())
            {
                await TryRefreshToken();
            }
            try
            {
                if (mlObject != null)
                {
                    var content = JsonConvert.SerializeObject(mlObject);

                    if (mlObject.topic == "items")
                    {

                    }
                    else if (mlObject.topic == "questions")
                    {
                        //Pega os detalhes 
                        var id = GetId(mlObject.resource);
                        ML_QUESTION_DETAIL_URL = String.Format(ML_QUESTION_DETAIL_URL, id, ML_ACCESS_TOKEN);
                        var response = await _client.GetAsync(ML_QUESTION_DETAIL_URL, HttpCompletionOption.ResponseContentRead);

                        if (response.StatusCode == HttpStatusCode.OK ||
                            response.StatusCode == HttpStatusCode.Accepted)
                        {
                            var result = await response.Content.ReadAsAsync<QuestionInfo>();

                            if (result != null)
                            {
                                var msg = String.Empty;

                                if (result.status == "ANSWERED")
                                {
                                    //await SendNotificationAsyncQuestion(result.seller_id.ToString(), "Sua pergunta foi respondida", result.answer.text);
                                }
                                else
                                {
                                    await SendNotificationAsyncQuestion(result.seller_id.ToString(), "Nova pergunta recebida", result.text);
                                }
                            }
                            content = JsonConvert.SerializeObject(result);
                        }

                        if (config["AppSettings:enviarEmailNotificacao"] == "1")
                        {
                            //_mailServices.Send()
                            /*
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("alexandre.dias.simoes@outlook.com");
                            mail.To.Add(new MailAddress("alexandre.dias.simoes@outlook.com"));
                            mail.Subject = "HttpPost ML";
                            mail.Body = content;

                            SmtpClient client = new SmtpClient();
                            client.Credentials = new System.Net.NetworkCredential("alexandre.dias.simoes@outlook.com", "Ads7ficq$");
                            client.EnableSsl = true;
                            client.Port = 587;
                            client.Host = "smtp-mail.outlook.com";
                            client.Send(mail);
                            */
                        }
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        private async Task TryRefreshToken()
        {
            var response = await _client.PostAsync(ML_REFRESH_TOKEN_URL, new StringContent(String.Empty));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userInfo = JsonConvert.DeserializeObject<MLAutorizationInfo>(await response.Content.ReadAsStringAsync());

                var webConfig = Startup.Configuration;
                
                webConfig["ml:token"] = userInfo.Access_Token;
                webConfig["ml:refreshToken"] = userInfo.Refresh_Token;
                webConfig["ml:userId"] = userInfo.Refresh_Token;
                webConfig["ml:keyExpires"] = DateTime.Now.AddSeconds(userInfo.Expires_In ?? 0).ToString();
                webConfig["ml:loginDate"] = DateTime.Now.ToString();

            }
        }

        private string GetId(string input)
        {
            return input.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        private static async Task SendNotificationAsyncQuestion(string userId, string title, string message)
        {
            //Debug.WriteLine("Processando notificacao " + userId + " msg " + message);
            //if (userId != "186403972" && userId != "186406682") return;

            Debug.WriteLine("Processando notificacao do seu usuario " + userId + " msg " + message);

            // Define the notification hub.
            NotificationHubClient hub =
                NotificationHubClient.CreateClientFromConnectionString(
                    "Endpoint=sb://meumercadolivre.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=GU2Z3pqiTyGwo9tTyh4p/3R3Vn/6RUkHwsn5r2iv5Tg=", "notifications");

            // Create an array of breaking news categories.
            var categories = new string[] { userId };

            try
            {
                // Define a Windows Store toast.
                var wnsToast = "<toast><visual><binding template=\"ToastText02\">"
                    + "<text id=\"1\">" + title
                    + "</text><text id=\"2\">" + message + "</text>"
                    + "</binding></visual></toast>";
                await hub.SendWindowsNativeNotificationAsync(wnsToast, categories);
            }
            catch (ArgumentException argEx)
            {
                // An exception is raised when the notification hub hasn't been 
                // registered for the iOS, Windows Store, or Windows Phone platform. 
                Debug.WriteLine("Erro notificacao " + argEx.Message);
            }
        }

        private static async Task SendNotificationAsyncDebugInfo(ML mlObject)
        {
            // Define the notification hub.
            NotificationHubClient hub =
                NotificationHubClient.CreateClientFromConnectionString(
                    "Endpoint=sb://meumercadolivre.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=GU2Z3pqiTyGwo9tTyh4p/3R3Vn/6RUkHwsn5r2iv5Tg=", "notifications");

            // Create an array of breaking news categories.
            var categories = new string[] { mlObject.user_id.ToString() };

            try
            {
                // Define a Windows Store toast.
                var wnsToast = "<toast><visual><binding template=\"ToastText01\">"
                    + "<text id=\"1\">" + mlObject.user_id + " - " + mlObject.topic
                    + "</text></binding></visual></toast>";
                await hub.SendWindowsNativeNotificationAsync(wnsToast, categories);
            }
            catch (ArgumentException)
            {
                // An exception is raised when the notification hub hasn't been 
                // registered for the iOS, Windows Store, or Windows Phone platform. 
            }
        }
    }
}

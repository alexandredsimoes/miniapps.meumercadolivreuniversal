using Microsoft.Azure.NotificationHubs;
using MyML.BackendServices.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace MyML.Web.Admin.Controllers
{
    public class NotificacoesController : ApiController
    {
        private string ML_QUESTION_DETAIL_URL = "https://api.mercadolibre.com/questions/{0}?access_token={1}";
        public const string ML_MY_ITEM_DETAIL = "https://api.mercadolibre.com/items/{0}?attributes=title";
        private string ML_ACCESS_TOKEN = "";
        private string ML_USER_ID = "";
        private string ML_REFRESH_TOKEN_URL = "https://api.mercadolibre.com/oauth/token?grant_type=refresh_token&client_id={0}&client_secret={1}&refresh_token={2}";
        private string ML_REFRESH_TOKEN = "";
        private string ML_API_KEY = "1pW3zr833brws5ePUbtfcQc52xzq1Ocq";
        private string ML_CLIENT_ID = "8765232316929095";
        private string NOTIFICATION_HUB = "";
        private string NOTIFICATION_CONNECTION_STRING = "";

        HttpClient _client;
        DataService _dataService;

        public NotificacoesController()
        {
            ML_ACCESS_TOKEN = ConfigurationManager.AppSettings["ml:token"];
            ML_USER_ID = ConfigurationManager.AppSettings["ml:userId"];
            ML_REFRESH_TOKEN = ConfigurationManager.AppSettings["ml:refreshToken"];
            NOTIFICATION_CONNECTION_STRING = ConfigurationManager.AppSettings["Microsoft.Azure.NotificationHubs.ConnectionString"];
            NOTIFICATION_HUB = ConfigurationManager.AppSettings["Microsoft.Azure.NotificationHubs.Hub"];


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
            var sellerId = String.Empty;
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
                        var url = String.Format(ML_QUESTION_DETAIL_URL, id, ML_ACCESS_TOKEN);
                        var response = await _client.GetAsync(url, HttpCompletionOption.ResponseContentRead);

                        if (response.StatusCode == HttpStatusCode.OK ||
                            response.StatusCode == HttpStatusCode.Accepted)
                        {
                            var result = await response.Content.ReadAsAsync<QuestionInfo>();

                            if (result != null)
                            {
                                sellerId = result.seller_id.ToString();

                                url = string.Format(ML_MY_ITEM_DETAIL, result.item_id);
                                response = await _client.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    var productInfo = await response.Content.ReadAsAsync<Item>();
                                    if (productInfo != null)
                                    {
                                        var msg = String.Empty;

                                        if (result.status == "ANSWERED")
                                        {
                                            //await SendNotificationAsyncQuestion(result.seller_id.ToString(), "Sua pergunta foi respondida", result.answer.text);
                                        }
                                        else
                                        {
                                            await SendNotificationAsyncQuestion(result.seller_id.ToString(), result.id.ToString(), productInfo.Title, productInfo.Id,
                                                result.text);
                                            if (ConfigurationManager.AppSettings["enviarEmailNotificacao"] == "1")
                                            {
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
                                            }
                                        }
                                    }
                                }
                            }
                            content = JsonConvert.SerializeObject(result);
                        }


                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                //return Ok();
                // An exception is raised when the notification hub hasn't been 
                // registered for the iOS, Windows Store, or Windows Phone platform. 
                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();
                try
                {
                    mail.From = new MailAddress("alexandre.dias.simoes@outlook.com");
                    mail.To.Add(new MailAddress("alexandre.dias.simoes@outlook.com"));
                    mail.Subject = "ERRO ---  HttpPost ML";
                    mail.Body = ex.Message + "\r\n" + ex.StackTrace;
                    
                    client.Credentials = new System.Net.NetworkCredential("alexandre.dias.simoes@outlook.com", "Ads7ficq$");
                    client.EnableSsl = true;
                    client.Port = 587;
                    client.Host = "smtp-mail.outlook.com";
                    client.Send(mail);                    
                }
                catch { }
                return InternalServerError();
            }
        }

        private async Task TryRefreshToken()
        {
            var response = await _client.PostAsync(ML_REFRESH_TOKEN_URL, new StringContent(String.Empty));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userInfo = JsonConvert.DeserializeObject<MLAutorizationInfo>(await response.Content.ReadAsStringAsync());

                Configuration webConfig = WebConfigurationManager.OpenWebConfiguration("~");
                webConfig.AppSettings.Settings["ml:token"].Value = userInfo.Access_Token;
                webConfig.AppSettings.Settings["ml:refreshToken"].Value = userInfo.Refresh_Token;
                webConfig.AppSettings.Settings["ml:userId"].Value = userInfo.Refresh_Token;
                webConfig.AppSettings.Settings["ml:keyExpires"].Value = DateTime.Now.AddSeconds(userInfo.Expires_In ?? 0).ToString();
                webConfig.AppSettings.Settings["ml:loginDate"].Value = DateTime.Now.ToString();
                webConfig.Save();
            }
        }

        private string GetId(string input)
        {
            return input.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        private async Task SendNotificationAsyncQuestion(string userId, string id, string title, long? productId, string message)
        {
            //Debug.WriteLine("Processando notificacao " + userId + " msg " + message);
            //if (userId != "210358765" && userId != "210358789") return;

#if DEBUG
            Debug.WriteLine("Processando notificacao do seu usuario " + userId + " msg " + message);
#endif

            // Define the notification hub.
            NotificationHubClient hub =
                NotificationHubClient.CreateClientFromConnectionString(
                    NOTIFICATION_CONNECTION_STRING, NOTIFICATION_HUB);

            // Create an array of breaking news categories.
            var categories = new string[] { userId };

            try
            {

                var toastXML = new StringBuilder();
                toastXML.AppendLine($"<toast launch=\"action=openQuestion&amp;questionId={id}&amp;itemId={productId}\">");
                toastXML.AppendLine("<visual>");
                toastXML.AppendLine("<binding template=\"ToastGeneric\">");
                toastXML.AppendLine($"<text hint-wrap=\"false\">{title}</text>");
                toastXML.AppendLine($"<text>{message}</text>");
                //toastXML.Append("<!--<image placement="appLogoOverride" hint-crop="circle" src="https://unsplash.it/64?image=1027"/>-->");
                //toastXML.Append("<image placement="hero" src="https://unsplash.it/360/180?image=1043"/>");
                toastXML.AppendLine("</binding>");
                toastXML.AppendLine("</visual>");
                toastXML.AppendLine("<actions>");
                toastXML.AppendLine("<input id=\"textBox\" type=\"text\" placeHolderContent=\"responder\"/>");
                toastXML.AppendLine("<action");
                toastXML.AppendLine("content=\"Responder\"");
                toastXML.AppendLine("activationType=\"background\"");
                toastXML.AppendLine($"arguments=\"action=enviar&amp;questionId={id}&amp;productId{productId}\"/>");
                toastXML.AppendLine("<action");
                toastXML.AppendLine("content=\"Cancelar\"");
                toastXML.AppendLine("activationType=\"background\"");
                toastXML.AppendLine("arguments=\"cancel\"/>");


                toastXML.AppendLine("</actions>");
                toastXML.AppendLine("</toast>");

#if DEBUG
                Debug.WriteLine(toastXML.ToString());
#endif
                await hub.SendWindowsNativeNotificationAsync(toastXML.ToString(), categories);
            }
            catch (Exception argEx)
            {
                throw argEx;
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

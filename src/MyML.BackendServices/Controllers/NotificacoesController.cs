using Microsoft.Azure;
using Microsoft.Azure.NotificationHubs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MyML.BackendServices;
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
        public const string ML_MY_ITEM_DETAIL = "https://api.mercadolibre.com/items/{0}?attributes=title,id";
        private string ML_ACCESS_TOKEN = "";
        private string ML_USER_ID = "";
        private string ML_REFRESH_TOKEN_URL = "https://api.mercadolibre.com/oauth/token?grant_type=refresh_token&client_id={0}&client_secret={1}&refresh_token={2}";
        private string ML_REFRESH_TOKEN = "";
        private string ML_API_KEY = "";
        private string ML_CLIENT_ID = "";
        private string NOTIFICATION_HUB = "";
        private string NOTIFICATION_CONNECTION_STRING = "";
        MailMessage _mail;
        SmtpClient _smtpClient;

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

            _mail = new MailMessage();
            _smtpClient = new SmtpClient();
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
                                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted)
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
                                            //var toastXML = new StringBuilder();
                                            //toastXML.AppendLine($"<toast launch=\"action=openQuestion&amp;questionId={id}&amp;itemId={productInfo.Id}\">");
                                            //toastXML.AppendLine("<visual>");
                                            //toastXML.AppendLine("<binding template=\"ToastGeneric\">");
                                            //toastXML.AppendLine($"<text hint-wrap=\"false\">{productInfo.Title.Replace("\"", "").Replace("&", "and").Replace("<", "").Replace(">", "")}</text>");
                                            //toastXML.AppendLine($"<text>{result.text.Replace("\"", "").Replace("&", "and").Replace("<", "").Replace(">", "")}</text>");                                            
                                            //toastXML.AppendLine("</binding>");
                                            //toastXML.AppendLine("</visual>");
                                            //toastXML.AppendLine("<actions>");
                                            //toastXML.AppendLine("<input id=\"textBox\" type=\"text\" placeHolderContent=\"responder\"/>");
                                            //toastXML.AppendLine("<action");
                                            //toastXML.AppendLine("content=\"Responder\"");
                                            //toastXML.AppendLine("activationType=\"background\"");
                                            //toastXML.AppendLine($"arguments=\"action=enviar&amp;questionId={id}&amp;productId{productInfo.Id}\"/>");
                                            //toastXML.AppendLine("<action");
                                            //toastXML.AppendLine("content=\"Cancelar\"");
                                            //toastXML.AppendLine("activationType=\"background\"");
                                            //toastXML.AppendLine("arguments=\"cancel\"/>");
                                            //toastXML.AppendLine("</actions>");
                                            //toastXML.AppendLine("</toast>");

                                            //lock (this)
                                            //{
                                            //    var cn = ConfigurationManager.AppSettings["table_connectionstring"];
                                            //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("table_connectionstring"));


                                            //    // Create the table client.
                                            //    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                                            //    // Create the CloudTable object that represents the "people" table.
                                            //    CloudTable table = tableClient.GetTableReference("notifications");

                                            //    // Create a new customer entity.
                                            //    NotificationMessage notification = new NotificationMessage(result.id.ToString(), result.seller_id.ToString());
                                            //    notification.Content = toastXML.ToString();
                                            //    notification.Processed = false;

                                            //    // Create the TableOperation object that inserts the customer entity.
                                            //    TableOperation insertOperation = TableOperation.Insert(notification);

                                            //    // Execute the insert operation.
                                            //    table.Execute(insertOperation);
                                            //}

                                            var r = await SendNotificationAsyncQuestion(result.seller_id.ToString(), result.id.ToString(), productInfo.Title, productInfo.Id,
                                                result.text);

                                            var jason = JsonConvert.SerializeObject(r);
                                            Debug.WriteLine($"NOTIFICACAO -> {result.seller_id} State ->{jason}");

                                            EnviarEmail("Resultado Notificação", jason);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                EnviarEmail("Erro HttpPost", ex.Message + "\r\n" + ex.StackTrace, true);
                return BadRequest();
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

        private async Task<NotificationOutcome> SendNotificationAsyncQuestion(string userId, string id, string title, string productId, string message)
        {
            if (String.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(id)
                || string.IsNullOrWhiteSpace(title) || productId == null
                || string.IsNullOrWhiteSpace(message))
            {
                return null;
            }
            //Debug.WriteLine("Processando notificacao " + userId + " msg " + message);
            // if (userId != "210358765" && userId != "210358789") return null;

#if DEBUG
            Debug.WriteLine("Processando notificacao do seu usuario " + userId + " msg " + message);
#endif


            var categories = new string[] { userId };

            var toastXML = new StringBuilder();
            try
            {


                toastXML.AppendLine($"<toast launch=\"action=openQuestion&amp;questionId={id}&amp;itemId={productId}\">");
                toastXML.AppendLine("<visual>");
                toastXML.AppendLine("<binding template=\"ToastGeneric\">");
                toastXML.AppendLine($"<text hint-wrap=\"false\">{title.Replace("\"", "").Replace("&", "and").Replace("<", "").Replace(">", "")}</text>");
                toastXML.AppendLine($"<text>{message.Replace("\"", "").Replace("&", "and").Replace("<", "").Replace(">", "")}</text>");
                //toastXML.Append("<!--<image placement="appLogoOverride" hint-crop="circle" src="https://unsplash.it/64?image=1027"/>-->");
                //toastXML.Append("<image placement="hero" src="https://unsplash.it/360/180?image=1043"/>");
                toastXML.AppendLine("</binding>");
                toastXML.AppendLine("</visual>");
                toastXML.AppendLine("<actions>");
                toastXML.AppendLine("<input id=\"textBox\" type=\"text\" placeHolderContent=\"responder\"/>");
                toastXML.AppendLine("<action");
                toastXML.AppendLine("content=\"Responder\"");
                toastXML.AppendLine("activationType=\"background\"");
                toastXML.AppendLine($"arguments=\"action=enviar&amp;questionId={id}&amp;productId={productId}\"/>");
                toastXML.AppendLine("<action");
                toastXML.AppendLine("content=\"Cancelar\"");
                toastXML.AppendLine("activationType=\"background\"");
                toastXML.AppendLine("arguments=\"cancel\"/>");
                toastXML.AppendLine("</actions>");
                toastXML.AppendLine("</toast>");



#if DEBUG
                Debug.WriteLine(toastXML.ToString());
#endif

                var r = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toastXML.ToString(), categories.AsEnumerable());
                var s = JsonConvert.SerializeObject(r);
                EnviarEmail("Envio OK", toastXML.ToString() + "\r\n\r\n" + s, false);
                return r;
            }
            catch (Exception argEx)
            {
                throw new Exception($"Erro no envio da notificacao\r\n\r\n{toastXML.ToString()}", argEx);
            }
        }

        private void EnviarEmail(string assunto, string conteudo, bool forcarEnvio = false)
        {
            if (ConfigurationManager.AppSettings["enviarEmailNotificacao"] == "1" || forcarEnvio)
            {
                try
                {
                    _mail.To.Clear();
                    _mail.From = new MailAddress("");
                    _mail.To.Add(new MailAddress(""));
                    _mail.Subject = "NOTIFICATIONS - " + assunto;
                    _mail.Body = conteudo;

                    _smtpClient.Credentials = new System.Net.NetworkCredential("", "");
                    _smtpClient.EnableSsl = true;
                    _smtpClient.Port = 587;
                    _smtpClient.Host = "smtp-mail.outlook.com";
                    _smtpClient.Send(_mail);
                }
                catch { }
            }
        }
    }
}

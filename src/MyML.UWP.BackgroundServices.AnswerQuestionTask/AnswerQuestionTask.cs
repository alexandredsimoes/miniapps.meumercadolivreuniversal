using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace MyML.UWP.BackgroundServices
{
    public sealed class AnswerQuestionTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //ApplicationData.Current.LocalSettings.Values["QUESTION_ID"] = details.Argument;
            var deferral = taskInstance.GetDeferral();

            try
            {
                
                var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
                if (details != null)
                {
                    
                    if(details.Argument.Contains("enviar"))
                    {
                        //Captura as informações da questão
                        var values = details.Argument.Split(new[] { '&' });
                        var questionId = values[1].Split(new[] { '=' })[1];
                        var mensagem = details.UserInput.FirstOrDefault(c => c.Key == "textBox").Value as string;

                        await AnswerQuestion(questionId, mensagem);
                    }                    
                    return;
                }
            }
            catch {/*Nada faz*/}
            finally
            {
                deferral.Complete();
            }
        }

        private async Task<bool> AnswerQuestion(string questionId, string content)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            var accessToken = ApplicationData.Current.LocalSettings.Values["ml_access_token"] as string;

            var url = $"https://api.mercadolibre.com/answers?access_token={accessToken}";

            //Monta os parametros
            var parametros = new
            {
                question_id = questionId,
                text = content
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var json = $"{{question_id:{questionId}, text:'{content}'}}";
            var response = await httpClient.PostAsync(url, new StringContent(json));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
    }
}




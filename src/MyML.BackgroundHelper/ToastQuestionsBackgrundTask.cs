using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Notifications;

namespace MyML.BackgroundHelper
{
    public sealed class ToastQuestionsBackgrundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            //Faz alguma coisa
            deferral.Complete();
            //try
            //{
            //    ApplicationData.Current.LocalSettings.Values["QUESTION_ID"] = "Apenas um teste";
            //}
            //catch (Exception ex)
            //{
                
            //}

            //return;
            //var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            //if (details == null)
            //{
            //    ApplicationData.Current.LocalSettings.Values["QUESTION_ID"] = details.Argument;
            //    Debug.WriteLine("NOTIFICACAO -> DETAILS == NULL");
            //    ///BackgroundTaskStorage.PutError("TriggerDetails was not ToastNotificationActionTriggerDetail.");
            //    return;
            //}

            //string arguments = details.Argument;
            //Debug.WriteLine($"NOTIFICACAO -> DETAILS.ARGUMENT{details.Argument}");
            //if (arguments == null || !arguments.Equals("quickReply"))
            //{

            //    //BackgroundTaskStorage.PutError($"Expected arguments to be 'quickReply' but was '{arguments}'.");
            //    return;
            //}

            //var answer = details.UserInput;
            //Debug.WriteLine($"NOTIFICACAO -> ANSWER == {details.UserInput}");

            
            
            //var userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            //var accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            //var url = Consts.GetUrl(Consts.ML_QUESTIONS_ANSWER_URL, accessToken);

            ////Monta os parametros
            //var parametros = new
            //{
            //    question_id = questionId,
            //    text = content
            //};



            //_httpClient.DefaultRequestHeaders.Accept.Clear();

            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



            //var json = new StringContent(JsonConvert.SerializeObject(parametros));
            //var response = await _httpClient.PostAsync(url, json);
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    return true;
            //}
            //return false;
            //BackgroundTaskStorage.PutAnswer(BackgroundTaskStorage.ConvertValueSetToApplicationDataCompositeValue(details.UserInput));

        }
    }
}

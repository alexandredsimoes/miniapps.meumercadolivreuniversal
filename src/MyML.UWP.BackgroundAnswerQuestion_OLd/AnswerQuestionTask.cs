using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace MyML.UWP.BackgroundAnswerQuestion
{
    public sealed class AnswerQuestionTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            //Faz alguma coisa
            deferral.Complete();
            try
            {
                ApplicationData.Current.LocalSettings.Values["QUESTION_ID"] = "Apenas um teste";
            }
            catch (Exception ex)
            {

            }
        }
    }
}

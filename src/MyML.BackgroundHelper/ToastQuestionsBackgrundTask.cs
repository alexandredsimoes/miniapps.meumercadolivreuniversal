using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;

namespace MyML.BackgroundHelper
{
    public sealed class ToastQuestionsBackgrundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the background task details
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            string taskName = taskInstance.Task.Name;

            Debug.WriteLine("Background " + taskName + " starting...");

            // Store the content received from the notification so it can be retrieved from the UI.
            RawNotification notification = (RawNotification)taskInstance.TriggerDetails;
            settings.Values[taskName] = notification.Content;

            Debug.WriteLine("Background " + taskName + " completed!");
        }
    }
}

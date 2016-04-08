using GalaSoft.MvvmLight.Ioc;
using Microsoft.WindowsAzure.Messaging;
using MyML.UWP.AppStorage;
using MyML.UWP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;

namespace MyML.UWP.Services
{
    public static class NotificationHelper
    {
        private static IDataService _dataService;
        private static ResourceLoader _resourceLoader;     
        public static async Task<bool> SubscribeNotification()
        {
            if (_dataService == null)
                _dataService = SimpleIoc.Default.GetInstance<IDataService>();

            if (_resourceLoader == null)
                _resourceLoader = SimpleIoc.Default.GetInstance<ResourceLoader>();

            try
            {
                var user_id = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                
#if DEBUG

                Debug.WriteLine("Inscrevendo na notificação " + user_id);
#endif
                //Verifica se já está inscrito
                //var expirationString = _dataService.GetMLConfig(Consts.ML_NOTIFICATION_EXPIRES);
                //DateTime expirationDate = DateTime.MinValue;
                //if (DateTime.TryParse(expirationString, out expirationDate))
                //{
                //    if (expirationDate > DateTime.Now)
                //        return true;
                //}
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                //Endpoint=sb://meumercadolivre.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=wOen194enfv8wsOo0V5GqJ2wAFf6gQbxPFQCgRzk01A=;EntityPath=universal                     
                //Endpoint=sb://meumercadolivre.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=wOen194enfv8wsOo0V5GqJ2wAFf6gQbxPFQCgRzk01A=;EntityPath=universal
                //Endpoint=sb://meumercadolivreuniversal.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=il6dtCewcwNTF4Am4bccLekIGtg0vM5xx+EU7BbKW3w=
                var hub = new NotificationHub("notificacoes", "Endpoint=sb://meumercadolivreuniversal.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=il6dtCewcwNTF4Am4bccLekIGtg0vM5xx+EU7BbKW3w=");
                var result = await hub.RegisterNativeAsync(channel.Uri, new string[] { user_id });
                _dataService.SaveConfig(Consts.ML_NOTIFICATION_EXPIRES, result.ExpiresAt.ToString());

#if DEBUG
                Debug.WriteLine("Inscrito - expira em " + result.ExpiresAt.ToString());

#endif
                await AppLogs.WriteInfo("LoginPageViewModel.SubscribeNotification()", $"Expiração: {result.ExpiresAt} - Id: {result.RegistrationId}");
                
                return true;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("LoginPageViewModel.SubscribeNotification()", ex);
                return false;
            }
        }
    }
}

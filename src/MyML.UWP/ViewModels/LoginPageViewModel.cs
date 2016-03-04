using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyML.UWP.Models;
using Windows.Storage;
using MyML.UWP.Services;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.Views;
using MyML.UWP.AppStorage;
using System.Diagnostics;
using Windows.Networking.PushNotifications;
using Microsoft.WindowsAzure.Messaging;

namespace MyML.UWP.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public readonly IMercadoLivreService _mercadoLivreServices;
        public readonly IDataService _dataService;
        public LoginPageViewModel(IMercadoLivreService services, IDataService dataService)
        {
            _mercadoLivreServices = services;
            _dataService = dataService;
            RevokeAccess = new RelayCommand(RevokeAccessExecute);
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(_dataService.IsAuthenticated())
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                else
                    NavigationService.Navigate(typeof(MainPage), "login");
            }
            else
            {
                try
                {
#if DEBUG
                    Debug.WriteLine("TENTANDO RESTAURAR TOKEN DO ML ************************ ");
#endif
                    //Tenta atualizar o token de autenticação, caso já tenha sido autenticado em algum momento.
                    var refreshToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
                    if (!String.IsNullOrWhiteSpace(refreshToken))
                    {
                        var login = await _mercadoLivreServices.TryRefreshToken();
                        if (login != null && !String.IsNullOrWhiteSpace(login.Refresh_Token))
                        {
#if DEBUG
                            Debug.WriteLine("LOGIN RESTAURADO ************************ ");
#endif
                            _dataService.SaveConfig(Consts.ML_CONFIG_KEY_USER_ID, login.user_id);
                            _dataService.SaveConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN, login.Refresh_Token);
                            _dataService.SaveConfig(Consts.ML_CONFIG_KEY_EXPIRES, DateTime.Now.AddSeconds(login.Expires_In ?? 0).ToString());
                            _dataService.SaveConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN, login.Access_Token);
                            _dataService.SaveConfig(Consts.ML_CONFIG_KEY_LOGIN_DATE, DateTime.Now.ToString());

                            if (NavigationService.CanGoBack)
                                NavigationService.GoBack();
                            else
                                NavigationService.Navigate(typeof(MainPage), "login");
                        }
                        else
                        {
                            //Se não conseguir restaurar o token, redireciona para o login
                            NavigationUrl = Consts.GetUrl(Consts.ML_URL_AUTHENTICATION, Consts.ML_CLIENT_ID, "");
                        }
                    }
                    else
                    {
                        //Se não conseguir restaurar o token, redireciona para o login
                        NavigationUrl = Consts.GetUrl(Consts.ML_URL_AUTHENTICATION, Consts.ML_CLIENT_ID, "");
                    }
                }
                catch (Exception ex)
                {
                    AppLogs.WriteError("MainPageViewModel RefreshToken", ex);
                }
            }                        
        }

        private void RevokeAccessExecute()
        {
            _mercadoLivreServices.RevokeAccess();
        }

        internal async Task SaveAuthenticationInfo(MLAutorizationInfo userInfo)
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[Consts.ML_CONFIG_KEY_ACCESS_TOKEN] = userInfo.Access_Token;
                ApplicationData.Current.LocalSettings.Values[Consts.ML_CONFIG_KEY_EXPIRES] = DateTime.Now.AddSeconds(userInfo.Expires_In ?? 0).ToString("dd/MM/yyyy HH:mm:ss");

                ApplicationData.Current.LocalSettings.Values[Consts.ML_CONFIG_KEY_USER_ID] = userInfo.user_id;
                ApplicationData.Current.LocalSettings.Values[Consts.ML_CONFIG_KEY_REFRESH_TOKEN] = userInfo.Refresh_Token;
                ApplicationData.Current.LocalSettings.Values[Consts.ML_CONFIG_KEY_LOGIN_DATE] = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                await SubscribeNotification(userInfo.user_id);
                Shell.SetBusy(false);

                
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                else
                    NavigationService.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("LoginPageViewModel.SaveAuthenticationInfo()", ex);
            }
        }


        private async Task SubscribeNotification(string user_id)
        {
            try
            {
#if DEBUG
                Debug.WriteLine("Inscrevendo na notificação " + user_id);
#endif
                //Verifica se já está inscrito
                var expirationString = _dataService.GetMLConfig(Consts.ML_NOTIFICATION_EXPIRES);
                DateTime expirationDate = DateTime.MinValue;
                if (DateTime.TryParse(expirationString, out expirationDate))
                {
                    if (expirationDate > DateTime.Now)
                        return;
                }
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                                                                
                var hub = new NotificationHub("notifications", "Endpoint=sb://meumercadolivre.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=KtkSa6kJWmjoOunjPZd4p/E0zRYe2cNnnre2zXb6zCs=");
                var result = await hub.RegisterNativeAsync(channel.Uri, new string[] { user_id });
                _dataService.SaveConfig(Consts.ML_NOTIFICATION_EXPIRES, result.ExpiresAt.ToString());

#if DEBUG
                Debug.WriteLine("Inscrito - expira em " + result.ExpiresAt.ToString());
#endif
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("LoginPageViewModel.SubscribeNotification()", ex);
            }

        }

        private string _NavigationUrl;
        public string NavigationUrl
        {
            get { return _NavigationUrl; }
            set { Set(() => NavigationUrl, ref _NavigationUrl, value); }
        }


        public RelayCommand RevokeAccess { get; private set; }                      
    }
}

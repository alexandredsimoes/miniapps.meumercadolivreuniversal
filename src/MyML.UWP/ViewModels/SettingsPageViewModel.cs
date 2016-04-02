using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Template10.Mvvm;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using MyML.UWP.Services;
using Windows.Networking.PushNotifications;
using Microsoft.WindowsAzure.Messaging;
using MyML.UWP.AppStorage;

namespace MyML.UWP.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly ResourceLoader _resourceLoader;
        public SettingsPageViewModel(IDataService dataService, ResourceLoader resourceLoader)
        {
            _resourceLoader = resourceLoader;
            _dataService = dataService;

            SettingsPartViewModel = new SettingsPartViewModel(_dataService, _resourceLoader);
        }

        public SettingsPartViewModel SettingsPartViewModel { get; } //= new SettingsPartViewModel(_dataService, _resourceLoader);
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings;
        private readonly IDataService _dataService;
        private readonly ResourceLoader _resourceLoader;

        public SettingsPartViewModel(IDataService dataService, ResourceLoader resourceLoader)
        {
            _dataService = dataService;
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                _settings = Services.SettingsServices.SettingsService.Instance;

            TrySigninNotifications = new RelayCommand<bool?>(TrySigninNotificationsExecute);
        }

        private async void TrySigninNotificationsExecute(bool? state)
        {
            if(state ?? false)
            {
                _settings.IsNotificationSigned  = await SubscribeNotification();
            }
            else
            {

            }
        }

        private async Task<bool> SubscribeNotification()
        {
            try
            {
                var user_id = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                if (!_dataService.IsAuthenticated())
                {
                    await new MessageDialog("Você precisa estar autenticado para se inscrever nas notificações",
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    return false;
                }
#if DEBUG

                Debug.WriteLine("Inscrevendo na notificação " + user_id);
#endif
                //Verifica se já está inscrito
                var expirationString = _dataService.GetMLConfig(Consts.ML_NOTIFICATION_EXPIRES);
                DateTime expirationDate = DateTime.MinValue;
                if (DateTime.TryParse(expirationString, out expirationDate))
                {
                    if (expirationDate > DateTime.Now)
                        return true;
                }
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                //Endpoint=sb://meumercadolivre.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=wOen194enfv8wsOo0V5GqJ2wAFf6gQbxPFQCgRzk01A=;EntityPath=universal                     
                //Endpoint=sb://meumercadolivre.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=wOen194enfv8wsOo0V5GqJ2wAFf6gQbxPFQCgRzk01A=;EntityPath=universal
                //Endpoint=sb://meumercadolivreuniversal.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=il6dtCewcwNTF4Am4bccLekIGtg0vM5xx+EU7BbKW3w=
                var hub = new NotificationHub("notificacoes", "Endpoint=sb://meumercadolivreuniversal.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=il6dtCewcwNTF4Am4bccLekIGtg0vM5xx+EU7BbKW3w=");
                var result = await hub.RegisterNativeAsync(channel.Uri, new string[] { user_id });
                _dataService.SaveConfig(Consts.ML_NOTIFICATION_EXPIRES, result.ExpiresAt.ToString());

#if DEBUG
                Debug.WriteLine("Inscrito - expira em " + result.ExpiresAt.ToString());
                return true;
#endif
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("LoginPageViewModel.SubscribeNotification()", ex);
                return false;
            }
        }

        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
        }

        private string _BusyText = "Please wait...";
        public string BusyText
        {
            get { return _BusyText; }
            set { Set(ref _BusyText, value); }
        }


        public void ShowBusy()
        {
            Views.Shell.SetBusy(true, _BusyText);
        }

        public void HideBusy()
        {
            Views.Shell.SetBusy(false);
        }

        private string _ReceiveNotifications;
        public string ReceiveNotifications
        {
            get { return _ReceiveNotifications; }
            set { Set(() => ReceiveNotifications, ref _ReceiveNotifications, value); }
        }

        public RelayCommand<bool?> TrySigninNotifications { get; private set; }
    }

    public class AboutPartViewModel : ViewModelBase
    {
        private readonly ResourceLoader _resourceLoader;
        public AboutPartViewModel()
        {
            _resourceLoader = SimpleIoc.Default.GetInstance<ResourceLoader>();

            QualifyApp = new RelayCommand(async () =>
            {
                var uri = new Uri(string.Format("ms-windows-store://review/?PFN={0}", Package.Current.Id.FamilyName));
                await Windows.System.Launcher.LaunchUriAsync(uri);
            });

            SendEmail = new RelayCommand(async () =>
            {
                EmailMessage message = new EmailMessage();
                message.Subject = _resourceLoader.GetString("SobreEmailAssunto");
                message.Body = _resourceLoader.GetString("SobreEmailBody");
                message.To.Add(new EmailRecipient(_resourceLoader.GetString("SobreEmail")));
                await EmailManager.ShowComposeNewEmailAsync(message);
            });           

            SendLog = new RelayCommand(SendLogExecute);
            RemoveAds = new RelayCommand(RemoverAdsExecute);
        }

       

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async void SendLogExecute()
        {
            var logExists = true;
            try
            {
                var f = await ApplicationData.Current.LocalFolder.GetFileAsync("AppLogs.txt");
            }
            catch (FileNotFoundException fnf)
            {
                logExists = false;
            }
            if (!logExists)
            {
                await new MessageDialog(_resourceLoader.GetString("MainPageViewModelNotLogMessage"), _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

           
            var dlg = new MessageDialog(_resourceLoader.GetString("MainPageViewModelMsgEmailLog"), _resourceLoader.GetString("ApplicationTitle"));
            dlg.Commands.Add(new UICommand(_resourceLoader.GetString("Yes"), async (o) =>
            {
                var email = new Windows.ApplicationModel.Email.EmailMessage()
                {
                    Body = "Segue em anexo meu log de erro.",
                    Subject = "Meu MercadoLivre Universal - Log de erro",
                };

                var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("AppLogs.txt");
                email.Attachments.Add(new EmailAttachment("AppLogs.txt", file));
                email.To.Add(new EmailRecipient("alexandre.dias.simoes@outlook.com", "Alexandre Dias Simões"));
                await EmailManager.ShowComposeNewEmailAsync(email);
            }));
            dlg.Commands.Add(new UICommand(_resourceLoader.GetString("No")));
            await dlg.ShowAsync();
        }

        private async void RemoverAdsExecute()
        {
#if DEBUG
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
#endif

            if (!licenseInformation.ProductLicenses[Consts.CONFIG_REMOVE_ADS_KEY].IsActive)
            {

                PurchaseResults result = null;
                Views.Shell.SetBusy(true);
                try
                {
                    result = await CurrentApp.RequestProductPurchaseAsync(Consts.CONFIG_REMOVE_ADS_KEY);
                    
                    if (result.Status == ProductPurchaseStatus.Succeeded)
                    {
                        await new MessageDialog(_resourceLoader.GetString("ConfigurationPageCompraEfetuada")).ShowAsync();
                        App.ExibirAds = false;
                        ApplicationData.Current.RoamingSettings.Values[Consts.CONFIG_REMOVE_ADS_KEY] = true;
                    }
                    else if(result.Status == ProductPurchaseStatus.NotPurchased ||
                        result.Status == ProductPurchaseStatus.NotFulfilled)
                    {
                        await new MessageDialog(_resourceLoader.GetString("ConfigurationPageCompraNaoEfetuada"), _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    }
                    else if(result.Status == ProductPurchaseStatus.AlreadyPurchased)
                    {
                        await new MessageDialog(_resourceLoader.GetString("ConfigurationPageCompraJaEfetuada")).ShowAsync();
                        App.ExibirAds = false;
                        ApplicationData.Current.RoamingSettings.Values[Consts.CONFIG_REMOVE_ADS_KEY] = true;
                    }
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                }
                finally
                {
                    Views.Shell.SetBusy(false);
                }
            }
            else
            {
                Views.Shell.SetBusy(false);
                await new MessageDialog(_resourceLoader.GetString("ConfigurationPageCompraJaEfetuada"), _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                App.ExibirAds = false;
                ApplicationData.Current.RoamingSettings.Values[Consts.CONFIG_REMOVE_ADS_KEY] = true;
            }
        }

        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://bing.com");
        public RelayCommand QualifyApp { get; private set; }
        public RelayCommand SendEmail { get; private set; }
        public RelayCommand SendLog { get; private set; }
        public RelayCommand RemoveAds { get; private set; }        
    }
}


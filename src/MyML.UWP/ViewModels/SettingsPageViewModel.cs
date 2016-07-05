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
        public SettingsPageViewModel()
        {
            _resourceLoader = SimpleIoc.Default.GetInstance<ResourceLoader>();
            _dataService = SimpleIoc.Default.GetInstance<IDataService>();

            SettingsPartViewModel = new SettingsPartViewModel();
        }

        public SettingsPartViewModel SettingsPartViewModel { get; } //= new SettingsPartViewModel(_dataService, _resourceLoader);
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings;
        private readonly IDataService _dataService;
        private readonly ResourceLoader _resourceLoader;

        public SettingsPartViewModel()
        {
            _resourceLoader = SimpleIoc.Default.GetInstance<ResourceLoader>();
            _dataService = SimpleIoc.Default.GetInstance<IDataService>();
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                _settings = Services.SettingsServices.SettingsService.Instance;

            TrySigninNotifications = new RelayCommand(TrySigninNotificationsExecute);
        }

        private async void TrySigninNotificationsExecute()
        {
            try
            {
                if (!_dataService.IsAuthenticated())
                {
                    await new MessageDialog("Você precisa estar autenticado para se inscrever nas notificações",
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    return;
                }
                var result = await NotificationHelper.SubscribeNotification();
                if (result)
                {
                    await new MessageDialog("Registro efetuado com sucesso.", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                }
                else
                {
                    await new MessageDialog("Não foi possível efetuar o registro para receber notificações. Envie seu log de erros para o desenvolvedor.", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                }

                Views.Shell.SetBusy(true, "Efetuando registro");
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
            
            //_settings.IsNotificationSigned = result;
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

        private bool? _ReceiveNotifications;
        public bool? ReceiveNotifications
        {
            get { return _ReceiveNotifications; }
            set { Set(() => ReceiveNotifications, ref _ReceiveNotifications, value); }
        }


        public bool? UseHighQualityImages
        {
            get { return _settings.UseHighQualityImages; }
            set { _settings.UseHighQualityImages = value; base.RaisePropertyChanged(); }
        }

        public RelayCommand TrySigninNotifications { get; private set; }
    }

    public class AboutPartViewModel : ViewModelBase
    {
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public AboutPartViewModel()
        {
            _resourceLoader = SimpleIoc.Default.GetInstance<ResourceLoader>();
            _dataService = SimpleIoc.Default.GetInstance<IDataService>();

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

            SendLog = new RelayCommand<string>(SendLogExecute);
            RemoveAds = new RelayCommand(RemoverAdsExecute);
        }

       

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async void SendLogExecute(string token)
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
                var body = $"Segue em anexo meu log de erros.{Environment.NewLine}" +
                $"Token de acesso: {_dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN)}{Environment.NewLine}" +
                $"Token de Atualizacao: {_dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN)}{Environment.NewLine}" +
                $"Data expiração do token: {_dataService.GetMLConfig(Consts.ML_CONFIG_KEY_EXPIRES)}";
                var email = new Windows.ApplicationModel.Email.EmailMessage()
                {
                    Body = (String.IsNullOrWhiteSpace(token) ? "Segue em anexo meu log de erro." : body),
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
        public RelayCommand<string> SendLog { get; private set; }
        public RelayCommand RemoveAds { get; private set; }        
    }
}


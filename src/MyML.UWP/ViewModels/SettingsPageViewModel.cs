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

namespace MyML.UWP.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings;

        public SettingsPartViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                _settings = Services.SettingsServices.SettingsService.Instance;
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


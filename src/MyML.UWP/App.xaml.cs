using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using MyML.UWP.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using MyML.UWP.Models;
using System.Linq;
using System.Diagnostics;
using Windows.UI.Xaml.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Threading;
using MyML.UWP.Services;
using MyML.UWP.AppStorage;
using Windows.Storage;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace MyML.UWP
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();

            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pt-BR";
            //System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("pt");
            //System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("pt");

            UnhandledException += App_UnhandledException;
            SplashFactory = (e) => new Views.Splash(e);            

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            //RequestedTheme = ApplicationTheme.Light;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                Exception exception = e.Exception;
                AppLogs.WriteError("App_UnhandledException", exception);
                if ((exception is XmlException || exception is NullReferenceException) && exception.ToString().ToUpper().Contains("INNERACTIVE"))
                {
                    Debug.WriteLine("Handled Inneractive exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if (exception is NullReferenceException && exception.ToString().ToUpper().Contains("SOMA"))
                {
                    Debug.WriteLine("Handled Smaato null reference exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if ((exception is System.IO.IOException || exception is NullReferenceException) && exception.ToString().ToUpper().Contains("GOOGLE"))
                {
                    Debug.WriteLine("Handled Google exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if (exception is ObjectDisposedException && exception.ToString().ToUpper().Contains("MOBFOX"))
                {
                    Debug.WriteLine("Handled Mobfox exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if ((exception is NullReferenceException || exception is XamlParseException) && exception.ToString().ToUpper().Contains("MICROSOFT.ADVERTISING"))
                {
                    Debug.WriteLine("Handled Microsoft.Advertising exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if ((exception is NullReferenceException || exception is XamlParseException) && exception.ToString().ToUpper().Contains("MICROSOFT.ADMEDIATOR"))
                {
                    Debug.WriteLine("Handled Microsoft.Advertising exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                
                if (Debugger.IsAttached)
                {
                    // An unhandled exception has occurred; break into the debugger
                    Debugger.Break();
                }

                e.Handled = true;
            }
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {

            //Armazena a quantidade de execuções do aplicativo
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Consts.CONFIG_KEY_EXECUCOES))
            {
                var qtde = 1;
                if (int.TryParse(ApplicationData.Current.LocalSettings.Values[Consts.CONFIG_KEY_EXECUCOES].ToString(), out qtde))
                {
                    if (qtde != -1)
                    {
                        qtde++;
                        ApplicationData.Current.LocalSettings.Values[Consts.CONFIG_KEY_EXECUCOES] = qtde;
                    }
                }
                else
                    ApplicationData.Current.LocalSettings.Values[Consts.CONFIG_KEY_EXECUCOES] = 1;
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values.Add(Consts.CONFIG_KEY_EXECUCOES, 1);
            }

            //Efetua a verificação da licença do usuário (remover ADS)
            VerifyLicense();

            // content may already be shell when resuming
            if ((Window.Current.Content as Views.Shell) == null)
            {
                // setup hamburger shell
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
                Window.Current.Content = new Views.Shell(nav);
            }

            DataService ds = new DataService();
            ds.Initialize();


            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }

            //DispatcherHelper.Initialize();
            //return Task.CompletedTask;
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            if (startKind == StartKind.Launch)
                await RegisterBackgroundTask();

            NavigationService.Navigate(typeof(Views.MainPage));
            //return Task.CompletedTask;
        }

        private void VerifyLicense()
        {
            //ApplicationData.Current.RoamingSettings.Values.Clear();

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(Consts.CONFIG_REMOVE_ADS_KEY))
                ExibirAds = !(bool)ApplicationData.Current.RoamingSettings.Values[Consts.CONFIG_REMOVE_ADS_KEY];
            else
                ExibirAds = true;
        }


        private void UnregisterTasks()
        {
            var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(i => i.Name.Equals(Consts.BACKGROUND_TASKNAME_QUESTIONS));

            if (task != null)
                task.Unregister(true);
        }

        private async Task<bool> RegisterBackgroundTask()
        {
            //Limpa qualquer notificação pendente
            ToastNotificationManager.History.Clear();

            // Unregister any previous exising background task
            UnregisterTasks();

            // Request access
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            // If denied
            if (status != BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity && status != BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                return false;

            // Construct the background task
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            {
                Name = Consts.BACKGROUND_TASKNAME_QUESTIONS,
                TaskEntryPoint = "MyML.BackgroundHelper.ToastQuestionsBackgrundTask"
            };

            // Set trigger for Toast History Changed
            builder.SetTrigger(new PushNotificationTrigger());

            // And register the background task
            BackgroundTaskRegistration registration = builder.Register();

            return true;
        }

        public static bool ExibirAds { get; internal set; }
    }
}


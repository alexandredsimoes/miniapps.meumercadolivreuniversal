using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using MyML.UWP.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using MyML.UWP.Models;
using System.Linq;
using System.Diagnostics;
using MyML.UWP.Services;
using MyML.UWP.AppStorage;
using Windows.Storage;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.UI;
using Microsoft.HockeyApp;
using MyML.UWP.Views;
using Template10.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Services.Store.Engagement;

namespace MyML.UWP
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {

            Microsoft.HockeyApp.HockeyClient.Current.Configure("046d5d06bf8e4703a1bdc2f201875c9a",
             new TelemetryConfiguration() { EnableDiagnostics = true });
            UnhandledException += App_UnhandledException;
            InitializeComponent();
            
            //System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("pt");
            //System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("pt");

            
            SplashFactory = (e) => new Views.Splash(e);

            RegistrarAplicativoNoDevCenter();

            using(var db = new MercadoLivreContext())
            {
                db.Database.Migrate();
            }
            #region App settings

            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            //RequestedTheme = ApplicationTheme.Light;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;

            switch (settings.SelectedCountry)
            {
                case "MLA":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-AR";
                    break;
                case "MLB":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pt-BR";
                    break;
                case "MBO":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-BO";
                    break;
                case "MEC":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-EC";
                    break;
                case "MPE":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-PE";
                    break;
                case "MCR":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-CR";
                    break;
                case "MLC":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-CL";
                    break;
                case "MCO":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-CO";
                    break;
                case "MPA":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-PA";
                    break;
                case "MCU":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-CU";
                    break;
                case "MSV":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-SV";
                    break;
                case "MNI":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-NI";
                    break;
                case "MPT":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pt-PT";
                    break;
                case "MGT":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-GT";
                    break;
                case "MHN":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-HN";
                    break;
                case "MLV":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-VE";
                    break;
                case "MLM":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-MX";
                    break;
                case "MRD":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-DO";
                    break;
                case "MLU":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-UY";
                    break;
                case "MPY":
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-PY";
                    break;
            }
            #endregion
        }

        private async void RegistrarAplicativoNoDevCenter()
        {
            await StoreServicesEngagementManager.GetDefault().RegisterNotificationChannelAsync();
        }

        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                var resourceLoader = ResourceLoader.GetForCurrentView();

                if (e.Exception != null)
                    await AppLogs.WriteError("App_UnhandledException", e.Exception);

                Shell.SetBusy(false);
                var dialog = new MessageDialog("Houve um erro inesperado", resourceLoader.GetString("ApplicationTitle"));
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            //Efetua a verificação da licença do usuário (remover ADS)
            VerifyLicense();

            if (!(Window.Current.Content is ModalDialog))
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };

                //if (args.Kind == ActivationKind.Launch &&
                //    args.PreviousExecutionState == ApplicationExecutionState.NotRunning)
                //{
                //    DataService ds = new DataService();
                //    ds.Initialize();
                //}

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    await statusBar.HideAsync();
                }

                //windows title bar      
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Color.FromArgb(100, 254, 220, 19);
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ForegroundColor =
                //    Colors.White;
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Color.FromArgb(100, 254, 220, 19);
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor =
                //    Colors.White;

                //StatusBar for Mobile

                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    //Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundColor = Color.FromArgb(100, 254, 220, 19);
                    //Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundOpacity = 1;
                    //Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
                }
            }
            await Task.CompletedTask;


            // content may already be shell when resuming
            //if ((Window.Current.Content as Views.Shell) == null)
            //{
            //    // setup hamburger shell
            //    var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            //    Window.Current.Content = new Views.Shell(nav);
            //}

            //DataService ds = new DataService();
            //ds.Initialize();


            //if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            //{
            //    var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            //    await statusBar.HideAsync();
            //}

            //DispatcherHelper.Initialize();
            //return Task.CompletedTask;
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            if (startKind == StartKind.Launch)
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


                await RegisterBackgroundTask();

                try
                {
                    // Install the main VCD. 
                    StorageFile vcdStorageFile =
                     await Package.Current.InstalledLocation.GetFileAsync(
                       @"MeuMercadoLivreUniversalCommands.xml");

                    await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.
                        InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);

                    // Update phrase list.
                    //ViewModel.ViewModelLocator locator = App.Current.Resources["ViewModelLocator"] as ViewModel.ViewModelLocator;
                    //if (locator != null)
                    //{
                    //  await locator.TripViewModel.UpdateDestinationPhraseList();
                    //}
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
                }
            }

            if (args.Kind == ActivationKind.ToastNotification)
            {
                //Verifica de onde veio esse toast/
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                if (toastArgs != null)
                {
#if DEBUG
                    var items = String.Empty;
                    toastArgs.UserInput.ToList().ForEach(c =>
                    {
                        items += $"{c.Key}={c.Value}\r\n";
                    });
                    await AppLogs.WriteInfo("Start", toastArgs.Argument);
                    await AppLogs.WriteInfo("Start", items);
#endif
                    if (toastArgs.Argument.Contains("openQuestion"))
                        NavigationService.Navigate(typeof(Views.Secure.PerguntasVendasPage));
                }
            }
            else if (args.Kind == ActivationKind.VoiceCommand)
            {
                // Event args can represent many different activation types. 
                // Cast it so we can get the parameters we care about out.
                var commandArgs = args as VoiceCommandActivatedEventArgs;

                if (commandArgs != null)
                {
                    Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;

                    var textoBusca = speechRecognitionResult.SemanticInterpretation.Properties["dictatedSearchTerms"].FirstOrDefault();
                    NavigationService.Navigate(typeof(Views.BuscaPage), textoBusca, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());                                        
                }
            }
            else
            {
                NavigationService.Navigate(typeof(Views.MainPage));
            }


            await Task.CompletedTask;
        }

        private void VerifyLicense()
        {
#if DEBUG
            ExibirAds = true;
            return;
#endif
            //ApplicationData.Current.RoamingSettings.Values.Clear();

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(Consts.CONFIG_REMOVE_ADS_KEY))
                ExibirAds = !(bool)ApplicationData.Current.RoamingSettings.Values[Consts.CONFIG_REMOVE_ADS_KEY];
            else
                ExibirAds = true;
        }


        private void UnregisterTasks()
        {
            var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(i => i.Name.Equals(Consts.BACKGROUND_TASKNAME_QUESTIONS));

            task?.Unregister(true);
        }

        private async Task<bool> RegisterBackgroundTask()
        {
            try
            {
                UnregisterTasks();

                await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = Consts.BACKGROUND_TASKNAME_QUESTIONS;
                //var t = new Windows.ApplicationModel.Background.SystemTrigger(SystemTriggerType.TimeZoneChange, false);
                ToastNotificationActionTrigger trigger = new ToastNotificationActionTrigger();
                taskBuilder.SetTrigger(trigger);


                taskBuilder.TaskEntryPoint = typeof(MyML.UWP.BackgroundServices.AnswerQuestionTask).FullName;// "MyML.UWP.BackgroundAnswerQuestion.AnswerQuestionTask";
                var registration = taskBuilder.Register();
                registration.Completed += (BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args) =>
                {
                    args.CheckResult();
                };
                return true;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MainPage.xaml.cs - RegisterBackgroundTask", ex);
                return false;
            }
            finally
            {

            }
            ////Limpa qualquer notificação pendente
            //ToastNotificationManager.History.Clear();

            //// Unregister any previous exising background task
            //UnregisterTasks();

            //// Request access
            //BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            //// If denied
            //if (status != BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity && status != BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            //    return false;

            //// Construct the background task
            //BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            //{
            //    Name = Consts.BACKGROUND_TASKNAME_QUESTIONS,                
            //    TaskEntryPoint = "MyML.UWP.BackgroundAnswerQuestion.AnswerQuestionTask"
            //};

            //// Set trigger for Toast History Changed
            //ToastNotificationActionTrigger trigger = new ToastNotificationActionTrigger();
            //builder.SetTrigger(trigger);

            //// And register the background task
            //BackgroundTaskRegistration registration = builder.Register();

            //return true;
        }

        public static bool ExibirAds { get; internal set; }
    }
}


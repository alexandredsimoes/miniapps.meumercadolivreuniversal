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
using Template10.Controls;

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

            InitializeComponent();

            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pt-BR";
            //System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("pt");
            //System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("pt");

            UnhandledException += App_UnhandledException;
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            //RequestedTheme = ApplicationTheme.Light;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;

            #endregion
        }

        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                var resourceLoader = ResourceLoader.GetForCurrentView();

                if (e.Exception != null)
                    AppLogs.WriteError("App_UnhandledException", e.Exception);

                var dialog = new MessageDialog("Houve um erro inesperado", resourceLoader.GetString("ApplicationTitle"));
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            /*

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
            */
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
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Color.FromArgb(100, 254, 220, 19);
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ForegroundColor =
                    Colors.White;
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Color.FromArgb(100, 254, 220, 19);
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor =
                    Colors.White;

                //StatusBar for Mobile

                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundColor = Color.FromArgb(100, 254, 220, 19);
                    Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundOpacity = 1;
                    Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
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

                    //Nunca executamos, cria o banco de dados
                    DataService ds = new DataService();
                    ds.Initialize();
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

                    // Get the name of the voice command and the text spoken. See AdventureWorksCommands.xml for
                    // the <Command> tags this can be filled with.
                    string voiceCommandName = speechRecognitionResult.RulePath[0];
                    string textSpoken = speechRecognitionResult.Text;

                    // The commandMode is either "voice" or "text", and it indictes how the voice command
                    // was entered by the user.
                    // Apps should respect "text" mode by providing feedback in silent form.
                    //string commandMode = this.SemanticInterpretation("commandMode", speechRecognitionResult);


                    //foreach (var item in speechRecognitionResult.SemanticInterpretation.Properties)
                    //{
                    //    items += $"{item.Key}={item.Value}";
                    //}

                    Debug.WriteLine("FALOW => " + speechRecognitionResult.SemanticInterpretation.Properties);
                    NavigationService.Navigate(typeof(Views.BuscaPage), voiceCommandName + "-" + textSpoken, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
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
                AppLogs.WriteError("MainPage.xaml.cs - RegisterBackgroundTask", ex);
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


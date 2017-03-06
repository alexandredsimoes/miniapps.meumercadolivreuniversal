using Template10.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using MyML.UWP.Services;
using GalaSoft.MvvmLight.Command;
using MyML.UWP.Views;
using System;
using MyML.UWP.Models;
using MyML.UWP.AppStorage;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Views.Secure;

namespace MyML.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IMercadoLivreService _mercadoLivreServices;
        private readonly ResourceLoader _resourceLoader;
        public MainPageViewModel(IDataService dataService, IMercadoLivreService mercadoLivreService,
            ResourceLoader resourceLoader)
        {
            _dataService = dataService;
            _mercadoLivreServices = mercadoLivreService;
            _resourceLoader = resourceLoader;

            SelecionarCategoria = new RelayCommand<object>(async(o) => {
                var obj = o as MLCategorySearchResult;
                if (obj != null)
                {
                    await NavigationService.NavigateAsync(typeof(CategoriaPage), obj.id);
                }
            });

            SelecionarProduto = new RelayCommand<object>(async o =>
            {
                var obj = o as MLItemHomeFeature;
                if (obj != null)
                {
                    await NavigationService.NavigateAsync(typeof(ProdutoDetalhePage), obj.item_id);
                }
            });

            GotoItem = new RelayCommand<string>((s) =>
            {
                NavigationService.Navigate(typeof(AnunciosPage), s);
            });

            LoadProduto = new RelayCommand<string>((parametro) =>
           {
               NavigationService.Navigate(typeof(ProdutoDetalhePage), parametro);
           });

            DoLogin = new RelayCommand(() => { NavigationService.Navigate(typeof(LoginPage)); });

            RevokeAccess = new RelayCommand(RevokeAccessExecute);

            DoSearch = new RelayCommand<object>((o) =>
            {
                NavigationService.Navigate(typeof(BuscaPage), o);   
            });

            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
                IsAuthenticated = true;            

        }

        private async void RevokeAccessExecute()
        {
            try
            {
                Shell.SetBusy(true, "Revogando autenticação...");
                if (await _mercadoLivreServices.RevokeAccess())
                {
                    await _dataService.DeleteConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                    await _dataService.DeleteConfig(Consts.ML_CONFIG_KEY_EXPIRES);
                    await _dataService.DeleteConfig(Consts.ML_CONFIG_KEY_LOGIN_DATE);
                    await _dataService.DeleteConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
                    await _dataService.DeleteConfig(Consts.ML_CONFIG_KEY_USER_ID);
                    IsAuthenticated = false;
                }
            }
            finally
            {
                Shell.SetBusy(false);
            }            
        }

        public RelayCommand<string> LoadProduto { get; private set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LoadHomeFeatures();
            //await new MessageDialog("QUESTION_DETAIL = " + ApplicationData.Current.LocalSettings.Values["QUESTION_ID"]).ShowAsync();
            if (state.Any())
            {
                state.Clear();
            }
                      
            await VerifyExecutions();

#if DEBUG
            //Apenas para testar com outros login

            //APP_USR-8765232316929095-070816-7ede7e9ec66cc6efed196cf96b4e2839__M_I__-162927607
            //TG-57800daee4b07d84f29ee5ab-162927607
            //07/08/2016 23:31:41

            //_dataService.SaveConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN, "APP_USR-8765232316929095-030519-6b0453cc1585922a201a360f6f8d9220__F_A__-148261232");
            //_dataService.SaveConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN, "TG-58bca08fe4b00f735318b779-148261232");
            //_dataService.SaveConfig(Consts.ML_CONFIG_KEY_LOGIN_DATE, "06/03/2017 02:34:40");

#endif
            //var categories = await _mercadoLivreServices.ListCategories("MLB");
        }

        private async void LoadHomeFeatures()
        {
            Items = await _mercadoLivreServices.ListFeaturedHomeItems();
            RaisePropertyChanged("Items");

            Categories = await _mercadoLivreServices.ListCategories(null);
            RaisePropertyChanged("Categories");
        }


        private async Task VerifyExecutions()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Consts.CONFIG_KEY_EXECUCOES))
            {
                var qtde = 1;
                if (int.TryParse(ApplicationData.Current.LocalSettings.Values[Consts.CONFIG_KEY_EXECUCOES].ToString(), out qtde))
                {
                    if (qtde == 3)
                    {                        
                        var dialog = new MessageDialog(_resourceLoader.GetString("MsgPerguntaAvaliacao"), _resourceLoader.GetString("ApplicationTitle"));
                        var confirmou = false;
                        var uiCmdYes = new UICommand(_resourceLoader.GetString("Yes"),
                            (result) =>
                            {
                                confirmou = true;
                            });

                        var uiCmdNo = new UICommand(_resourceLoader.GetString("No"),
                            (result) =>
                            {
                                confirmou = false;
                            });

                        dialog.Commands.Add(uiCmdYes);
                        dialog.Commands.Add(uiCmdNo);
                        await dialog.ShowAsync(); //Exibe a pergunta de avaliação do produto

                        if(confirmou)
                        {
                            var uri = new Uri(string.Format("ms-windows-store://review/?PFN={0}", Package.Current.Id.FamilyName));
                            await Windows.System.Launcher.LaunchUriAsync(uri);                             
                                                       
                            //Se o usuário avaliou o aplicativo, reseta a configuração para não exibir mais a mensagem para ele
                            ApplicationData.Current.LocalSettings.Values[Consts.CONFIG_KEY_EXECUCOES] = -1;
                        }
                        else
                        {
                            //Se o usuário recusar, pergunta se deseja entrar em contato via e-mail
                            dialog.Content = _resourceLoader.GetString("MsgPerguntaFeedback");
                            uiCmdYes.Invoked = async (a) =>
                            {
                                EmailMessage message = new EmailMessage();
                                message.Subject = _resourceLoader.GetString("SobreEmailAssunto");
                                message.Body = _resourceLoader.GetString("SobreEmailBody");
                                message.To.Add(new EmailRecipient(_resourceLoader.GetString("SobreEmail")));
                                await EmailManager.ShowComposeNewEmailAsync(message);
                            };

                            await dialog.ShowAsync();
                            ApplicationData.Current.LocalSettings.Values[Consts.CONFIG_KEY_EXECUCOES] = 0;
                        }                            
                    }
                }
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {

            }

            Shell.SetBusy(false);
            return Task.CompletedTask;
        }


        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            return Task.CompletedTask;
        }

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        public IReadOnlyCollection<MLItemHomeFeature> Items { get; set; }
        public IReadOnlyCollection<MLCategorySearchResult> Categories { get; set; }

        public async void Logoff()
        {
            var result = await _mercadoLivreServices.RevokeAccess();
        }     


        private bool _IsAuthenticated = false;
        public bool IsAuthenticated
        {
            get { return _IsAuthenticated; }
            set { Set(() => IsAuthenticated, ref _IsAuthenticated, value); }
        }

        

        public RelayCommand RevokeAccess { get; private set; }
        public RelayCommand DoLogin { get; private set; }
        public RelayCommand<string> GotoItem { get; private set; }
        public RelayCommand<object> DoSearch { get; private set; }
        public RelayCommand<object> SelecionarProduto { get; private set; }
        public RelayCommand<object> SelecionarCategoria { get; private set; }
    }
}


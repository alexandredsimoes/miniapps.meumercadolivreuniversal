using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.Models.Mercadolivre;
using GalaSoft.MvvmLight.Command;
using MyML.UWP.Views;
using MyML.UWP.Views.Secure;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.DataTransfer;
using MyML.UWP.AppStorage;
using Windows.System;
using Template10.Services.NavigationService;
using Template10.Utils;
using System.Diagnostics;

namespace MyML.UWP.ViewModels
{
    public class ProdutoDetalheViewModel : ViewModelBase
    {
        private DataTransferManager _dataTransferManager = null;
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly IDataService _dataService;
        private readonly ResourceLoader _resourceLoader;

        public event Action<bool> ZoomModeChanged;

        public ProdutoDetalheViewModel(IMercadoLivreService mercadoLivreService, IDataService dataService, ResourceLoader resourceLoader)
        {
            _mercadoLivreService = mercadoLivreService;
            _dataService = dataService;
            _resourceLoader = resourceLoader;

            Perguntar = new RelayCommand(() =>
            {
                NavigationService.Navigate(typeof(PerguntarPage), SelectedProduct.id);
            });

            BuyItem = new RelayCommand<string>(BuyItemExecute);

            DoFavorite = new RelayCommand(async () =>
            {
                if (!_dataService.IsAuthenticated())
                {
                    await new MessageDialog(_resourceLoader.GetString("NotAuthenticatedMessage"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
                    NavigationService.Navigate(typeof(LoginPage), "favorite");
                    return;
                }
                if (SelectedProduct != null)
                {
                    var success = false;
                    if (!IsFavorite)
                        success = await _mercadoLivreService.BookmarkItem(SelectedProduct.id);
                    else
                        success = await _mercadoLivreService.RemoveBookmarkItem(SelectedProduct.id);

                    if (success)
                        IsFavorite = !IsFavorite;
                }
            });

            ShareProduct = new RelayCommand(() =>
            {
                try
                {
                    Shell.SetBusy(true, "Carregando informações");
                    DataTransferManager.ShowShareUI();
                }
                catch (Exception ex)
                {
                    AppLogs.WriteError("ShareProduct Command", ex);
                }
                finally
                {
                    Shell.SetBusy(false);
                }
            });

            OpenOptions = new RelayCommand<string>((o) =>
            {
                if (o == "seller_info")
                {
                    NavigationService.Navigate(typeof(VendedorInfoPage), SelectedProduct.seller_id);
                }
                else if (o == "product_description")
                {
                    NavigationService.Navigate(typeof(ProdutoDescricaoPage), SelectedProduct.id);
                }
                else if (o == "questions")
                {
                    NavigationService.Navigate(typeof(ProdutoPerguntasPage), SelectedProduct.id);
                }
            });

            OpenShipping = new RelayCommand(() =>
            {
                NavigationService.Navigate(typeof(ProdutoDetalheEnvioPage), SelectedProduct.id);
            });

            ShowZoom = new RelayCommand(() =>
            {
                ZoomMode = !ZoomMode;
                var handler = ZoomModeChanged;
                if (handler != null) handler(ZoomMode);
            });
        }

        private async void BuyItemExecute(string origin)
        {
            if (origin == "web")
            {
                var uri = new Uri(SelectedProduct.permalink);
                await Launcher.LaunchUriAsync(uri);
            }
            else
            {
#if !DEBUG
                await new MessageDialog("As compras pelo aplicativo não estão ativos devido a falta do recurso na API do MercadoLivre para aplicativos de terceiros.", "Opção não disponível").ShowAsync();
                return;
#endif
                await new MessageDialog("A compra pelo aplicativo não está disponível, devido a falta do recurso na API do MercadoLivre para aplicativos de terceiros.", "Opção não disponível").ShowAsync();
                return;
                if (!_dataService.IsAuthenticated())
                {
                    await new MessageDialog(_resourceLoader.GetString("NotAuthenticatedMessage"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
                    NavigationService.Navigate(typeof(LoginPage), "buyitem");
                    return;
                }

                NavigationService.Navigate(typeof(ComprarItemPage), SelectedProduct.id,
                    new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
            }
        }
        void _dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.Properties.Title = SelectedProduct.title;
            args.Request.Data.Properties.Description = SelectedProduct.title;
            args.Request.Data.Properties.ApplicationName = Windows.ApplicationModel.Package.Current.DisplayName;
            args.Request.Data.SetApplicationLink(new Uri(SelectedProduct.permalink));
            args.Request.Data.SetWebLink(new Uri(SelectedProduct.permalink));
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            if (_dataTransferManager != null)
                _dataTransferManager.DataRequested += _dataTransferManager_DataRequested;

            if (mode != NavigationMode.Back)
            {
                if (parameter != null)
                {
                    var id = parameter.ToString();
                    if (SelectedProduct == null || SelectedProduct.id != id)
                        await LoadProductDetails(parameter);
                }
            }
            //return base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            NavigationService.Frame.BackStack.ForEach(c =>
            {
                Debug.WriteLine("Historico -> " + c.SourcePageType);
            });

            if (!args.Suspending && ZoomMode)
            {
                args.Cancel = true;

                var handler = ZoomModeChanged;
                if (handler != null) handler(ZoomMode);
                ZoomMode = false;
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            if (_dataTransferManager != null)
                _dataTransferManager.DataRequested -= _dataTransferManager_DataRequested;

            Views.Shell.SetBusy(false);

            return Task.CompletedTask;
        }

        private async Task LoadProductDetails(object parameter)
        {
            try
            {
                Shell.SetBusy(true, "Carregando informações");
                SelectedProduct = await _mercadoLivreService.GetItemDetails(parameter.ToString(), new KeyValuePair<string, string>[] { });

                if (SelectedProduct != null)
                {
                    //Pega os dados do vendedor
                    var sellerId = SelectedProduct.seller == null ? SelectedProduct.seller_id : SelectedProduct.seller.id;
                    SellerInfo = await _mercadoLivreService.GetUserInfo(sellerId.ToString(),
                        new KeyValuePair<string, string>[] { /*new KeyValuePair<string, string>("attributes", "seller_reputation,points")*/ });


                    var favorites = await _mercadoLivreService.GetBookmarkItems();
                    if (favorites != null)
                        SelectedProduct.IsFavorite = favorites.Where(c => c.item_id == SelectedProduct.id).FirstOrDefault() != null;
                    else
                        SelectedProduct.IsFavorite = false;


                    IsFavorite = SelectedProduct.IsFavorite; //Para atualizar a UI

                    //Carrega as perguntas (apenas uma e seu total)
                    QuestionsList = await _mercadoLivreService.ListQuestionsByProduct(SelectedProduct.id, 0, 0, new KeyValuePair<string, object>[] {
                        new KeyValuePair<string, object>("attributes", "total,questions&limit=1&offset")
                    });

                    if (QuestionsList != null)
                    {
                        Questions = QuestionsList.total ?? 0;
                        HasQuestions = Questions > 0;
                        RaisePropertyChanged(() => HasQuestions);
                    }

                    //Verifica as opções de frete
                    ShippingExcludeRegions = null;
                    if (SelectedProduct.shipping?.free_shipping == true)
                    {
                        var excludeRegions = "";
                        if (SelectedProduct.shipping.free_methods != null)
                        {
                            //Verifica se existe zonas de exclusão
                            foreach (var excluded in SelectedProduct.shipping.free_methods)
                            {
                                if (excluded.rule != null)
                                {
                                    if (excluded.rule.free_mode == "exclude_region")
                                    {
                                        for (int i = 0; i < excluded.rule.value.Count; i++)
                                        {
                                            var region = excluded.rule.value[i];
                                            switch (region)
                                            {
                                                case "BR-NO":
                                                    region = "Norte";
                                                    break;
                                                case "BR-NE":
                                                    region = "Nordeste";
                                                    break;
                                                case "BR-SE":
                                                    region = "Sudeste";
                                                    break;
                                                case "BR-SO":
                                                    region = "Sul";
                                                    break;
                                                default:
                                                    region = excluded.rule.value[i];
                                                    break;
                                            }
                                            excludeRegions += region + (i < excluded.rule.value.Count - 1 ? ", " : "");
                                        }
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(excludeRegions))
                            ShippingExcludeRegions = $"(Exceto para as regiões {excludeRegions})";
                    }
                    else
                    {
                        if (SelectedProduct.shipping != null && SelectedProduct.shipping.id != null)
                        {
                            var shippingInfo = await _mercadoLivreService.GetShippingDetails(SelectedProduct.shipping.ToString());
                        }
                    }
                }
                else
                {
                    //Avisa o usuário?
                    await new MessageDialog("Ocorreu um erro ao carregar dados do produto. Verifique sua conexão de internet.", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                }
            }
            finally
            {
                Shell.SetBusy(false);
            }
        }

        private Item _SelectedProduct;
        public Item SelectedProduct
        {
            get
            {
                return _SelectedProduct;
            }
            set
            {
                Set(() => SelectedProduct, ref _SelectedProduct, value);
            }
        }

        private MLUserInfoSearchResult _SellerInfo;
        public MLUserInfoSearchResult SellerInfo
        {
            get
            {
                return _SellerInfo;
            }
            set
            {
                Set(() => SellerInfo, ref _SellerInfo, value);
            }
        }


        private bool _IsFavorite;
        public bool IsFavorite
        {
            get
            {
                return _IsFavorite;
            }
            set
            {
                Set(() => IsFavorite, ref _IsFavorite, value);
            }
        }

        private ProductQuestion _QuestionsList;
        public ProductQuestion QuestionsList
        {
            get { return _QuestionsList; }
            set { Set(() => QuestionsList, ref _QuestionsList, value); }
        }

        private int _Questions;
        public int Questions
        {
            get { return _Questions; }
            set { Set(() => Questions, ref _Questions, value); }
        }

        private string _ShippingExcludeRegions;

        public string ShippingExcludeRegions
        {
            get { return _ShippingExcludeRegions; }
            set { Set(() => ShippingExcludeRegions, ref _ShippingExcludeRegions, value); }
        }

        public bool HasQuestions { get; set; } = false;

        private bool _ZoomMode = false;
        public bool ZoomMode
        {
            get { return _ZoomMode; }
            set { Set(() => ZoomMode, ref _ZoomMode, value); }
        }



        public RelayCommand Perguntar { get; private set; }
        public RelayCommand DoFavorite { get; private set; }
        public RelayCommand ShareProduct { get; private set; }
        public RelayCommand<string> BuyItem { get; private set; }
        public RelayCommand<string> OpenOptions { get; set; }
        public RelayCommand OpenShipping { get; set; }
        public RelayCommand ShowZoom { get; private set; }
    }
}

using GalaSoft.MvvmLight.Command;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.Services.SettingsServices;
using Template10.Services.NavigationService;

namespace MyML.UWP.ViewModels
{
    public class VenderPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreServices;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        private IReadOnlyCollection<MLCategorySearchResult> _backupCategories;
        private readonly SettingsService _settings = SettingsService.Instance;

        public event Action<int> PartChanged;

        public VenderPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreServices = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;

            ProductInfo = new SellItem();

            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {

            }

            GetImage = new RelayCommand<string>(GetImageExecute);
            NextPart = new RelayCommand(NextPartExecute);
            SelectCategory = new RelayCommand<object>(SelectCategoryExecute);
            SelectPrice = new RelayCommand<object>(SelectPriceExecute);
            OpenCategories = new RelayCommand(() =>
            {
                IsCategoryOpen = !IsCategoryOpen;
            });
            OpenTypes = new RelayCommand(() =>
            {
                IsTypeOpen = !IsTypeOpen;
            });
            CreateItem = new RelayCommand(CreateItemExecute);
        }

        private async void CreateItemExecute()
        {
            try
            {

                Views.Shell.SetBusy(true, "Criando novo anúncio");

                ProductInfo.ListType = new MLListType() { id = SelectedListingPrice.listing_type_id };
                ProductInfo.ProductCategory = SelectedCategory.id;
                ProductInfo.BuyingMode = "buy_it_now"; //Sempre será esse?


                if (ProductInfo.ListType.id == "free" && ProductInfo.Quantity > 1)
                {
                    Views.Shell.SetBusy(false);
                    await Dispatcher.DispatchAsync(async() =>
                    {
                        await new MessageDialog("Quando o tipo de anúncio é GRÁTIS, a quantidade não pode ser maior que 1. Por favor, volte o corrija quantidade",
                            _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    });
                    
                    return;
                }

                ////Valida o anuncio antes
                var result = await _mercadoLivreServices.ValidateNewItem(_ProductInfo);

                if (result != null)
                {
                    var message = new StringBuilder();

                    foreach (var item in result.cause)
                    {
                        message.Append($"{item.code}-{item.message}{Environment.NewLine}");
                    }
                    await new MessageDialog($"Ocorreu o seguinte erro:{Environment.NewLine}{message.ToString()}",
                                                _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    return;
                }

                var r = await _mercadoLivreServices.ListNewItem(_ProductInfo);

                if (r != null)
                {
                    Views.Shell.SetBusy(true, "Enviando as imagens");
                    //Envia as imagens
                    foreach (var item in ProductInfo.Images)
                    {
                        var imagemInfo = await _mercadoLivreServices.UploadProductImage(item);
                        if (imagemInfo != null)
                        {
                            await _mercadoLivreServices.AddPicture(imagemInfo.id, r.id);
                        }
                    }

                    Views.Shell.SetBusy(false);
                    await new MessageDialog("Seu produto foi cadastrado com sucesso e em breve será listado.",
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                }
                else
                {
                    Views.Shell.SetBusy(false);
                    await new MessageDialog("Não foi possível criar seu item no momento. Envie o LOG de erros para o desenvolvedor em configurações -> Sobre -> Enviar Log.",
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                }
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        private void SelectPriceExecute(object obj)
        {
            var item = obj as MLListPrice;
            if (item == null) return;

            SelectedListingPrice = item;
            IsTypeOpen = false;
        }

        private async void SelectCategoryExecute(object obj)
        {
            var item = obj as MLCategorySearchResult;
            if (item == null) return;

            //pega os detalhes da categoria
            var categoryDetail = await _mercadoLivreServices.GetCategoryDetail(item.id);
            if (categoryDetail == null) return;


            if (categoryDetail.path_from_root.Count == 1)
                SelectedCategoryLevel = categoryDetail.name;
            else
            {
                SelectedCategoryLevel = string.Empty;
                foreach (var category in categoryDetail.path_from_root)
                {
                    SelectedCategoryLevel = (!String.IsNullOrWhiteSpace(SelectedCategoryLevel)  ? SelectedCategoryLevel + " -> " : "")  + category.name;
                }
            }

            if (categoryDetail.children_categories.Count > 0)
            {
                //Recarrega os subitems
                this.Categories.Clear();
                foreach (var category in categoryDetail.children_categories)
                {
                    this.Categories.Add(new MLCategorySearchResult()
                    {
                        id = category.id,
                        name = category.name,
                        total_items_in_this_category = category.total_items_in_this_category
                    });
                }

            }
            else
            {
                //Ultimo nivel, seleciona a categoria
                SelectedCategory = item;
                IsCategoryOpen = false;
                this.Categories = new ObservableCollection<MLCategorySearchResult>(_backupCategories);
            }
        }


        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = ActualPart > 1 && args.NavigationMode == NavigationMode.Back;
            if (args.Cancel)
            {
                ActualPart--;
                var handler = PartChanged;
                if (handler != null) handler(ActualPart);
            }
            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            AvailablePhotos = MaxImages - ProductInfo.Images.Count;
            return Task.CompletedTask;
        }


        private async void GetImageExecute(string source)
        {
            if (ProductInfo.Images.Count >= 5)
            {
                await new MessageDialog(_resourceLoader.GetString("SellProductPageMaxImages"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                return;
            }
            if (source == "Camera")
            {
                Windows.Media.Capture.CameraCaptureUI ui = new Windows.Media.Capture.CameraCaptureUI();
                ui.PhotoSettings.AllowCropping = true;

                var file = await ui.CaptureFileAsync(Windows.Media.Capture.CameraCaptureUIMode.Photo);

                //Tratamos a imagem aqui
                await ProcessImage(file);
            }
            else
            {
                FileOpenPicker fo = new FileOpenPicker();
                fo.CommitButtonText = "Selecionar imagem";
                fo.FileTypeFilter.Add(".jpg");
                fo.FileTypeFilter.Add(".png");
                fo.ViewMode = PickerViewMode.Thumbnail;
                var file = await fo.PickSingleFileAsync();

                //Tratamos a imagem aqui
                await ProcessImage(file);
            }

            AvailablePhotos = MaxImages - ProductInfo.Images.Count;
        }

        private async void NextPartExecute()
        {
            if (ActualPart == 3 && String.IsNullOrWhiteSpace(ProductInfo.Title))
            {
                await new MessageDialog("Informe o NOME do seu produto", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

            if (ActualPart == 4 && (ProductInfo.ProductValue ?? 0) <= 0)
            {
                await new MessageDialog("Informe o PREÇO do seu produto", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

            if (ActualPart == 5 && (ProductInfo.Quantity ?? 0) <= 0)
            {
                await new MessageDialog("Informe o QUANTIDADE disponível do seu produto", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

            if (ActualPart == 7 && (SelectedCategory == null))
            {
                await new MessageDialog("Informe o CATEGORIA do seu produto", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

            if (ActualPart == 8 && (ProductInfo.IsNew == null))
            {
                await new MessageDialog("Informe o ESTADO do seu produto", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

            if (ActualPart == 9 && SelectedListingPrice == null)
            {
                await new MessageDialog("Selecione o tipo de anúncio.", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }


            ActualPart++;

            var handler = PartChanged;
            handler?.Invoke(ActualPart);


            if (ActualPart == 7)
            {
                if (SelectedCategory == null)
                {
                    var settings = SettingsService.Instance;
                    try
                    {
                        Views.Shell.SetBusy(true, "carregando as categorias");

                        var lista = await _mercadoLivreServices.ListCategories(settings.SelectedCountry);
                        if (lista != null)
                        {
                            Categories = new ObservableCollection<MLCategorySearchResult>(lista);
                            _backupCategories = lista;
                        }
                        else
                        {
                            await new MessageDialog("Não foi possível acessar o serviço do MercadoLivre, verifique se você está conectado a internet.",
                                _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                        }
                    }
                    finally
                    {
                        Views.Shell.SetBusy(false);
                    }
                }
            }
            else if (ActualPart == 9)
            {
                try
                {
                    Views.Shell.SetBusy(true, "carregando os tipos de anúncio");
                    //Carrega os tipos de anuncio
                    ListingPrice = await _mercadoLivreServices.GetListingPrices(_settings.SelectedCountry, ProductInfo.ProductValue ?? 0,
                        new KeyValuePair<string, object>[] { });

                    IsTypeOpen = false;
                }
                finally
                {
                    Views.Shell.SetBusy(false);
                }
            }
        }

        private async Task ProcessImage(Windows.Storage.StorageFile file)
        {
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelHeight = 100;
                    //bitmapImage.DecodePixelWidth = 100;

                    await bitmapImage.SetSourceAsync(fileStream);
                    ProductInfo.Images.Add(new ProductImage() { Source = bitmapImage, LocalPath = file.Path });
                }
            }
        }

        private int _ActualPart = 1;
        public int ActualPart
        {
            get { return _ActualPart; }
            set { Set(() => ActualPart, ref _ActualPart, value); }
        }

        private int _AvailablePhotos;
        public int AvailablePhotos
        {
            get { return _AvailablePhotos; }
            set { Set(() => AvailablePhotos, ref _AvailablePhotos, value); }
        }

        private bool _IsCategoryOpen = false;
        public bool IsCategoryOpen
        {
            get { return _IsCategoryOpen; }
            set { Set(() => IsCategoryOpen, ref _IsCategoryOpen, value); }
        }

        private bool _IsTypeOpen = false;
        public bool IsTypeOpen
        {
            get { return _IsTypeOpen; }
            set { Set(() => IsTypeOpen, ref _IsTypeOpen, value); }
        }

        private SellItem _ProductInfo;
        public SellItem ProductInfo
        {
            get { return _ProductInfo; }
            set { Set(() => ProductInfo, ref _ProductInfo, value); }
        }

        private MLCategorySearchResult _SelectedCategory;
        public MLCategorySearchResult SelectedCategory
        {
            get { return _SelectedCategory; }
            set { Set(() => SelectedCategory, ref _SelectedCategory, value); }
        }

        private string _SelectedCategoryLevel;

        public string SelectedCategoryLevel
        {
            get { return _SelectedCategoryLevel; }
            set { Set(() => SelectedCategoryLevel, ref _SelectedCategoryLevel, value); }
        }



        private MLListPrice _SelectedListingPrice;
        public MLListPrice SelectedListingPrice
        {
            get { return _SelectedListingPrice; }
            set { Set(() => SelectedListingPrice, ref _SelectedListingPrice, value); }
        }

        private IReadOnlyList<MLListPrice> _ListingPrice;
        public IReadOnlyList<MLListPrice> ListingPrice
        {
            get { return _ListingPrice; }
            set { Set(() => ListingPrice, ref _ListingPrice, value); }
        }

        private ObservableCollection<MLCategorySearchResult> _Categories;
        public ObservableCollection<MLCategorySearchResult> Categories
        {
            get
            {
                return _Categories;
            }
            set
            {
                Set(() => Categories, ref _Categories, value);
            }
        }

        public int MaxImages { get; set; } = 5;



        public RelayCommand NextPart { get; private set; }
        public RelayCommand OpenCategories { get; private set; }
        public RelayCommand OpenTypes { get; private set; }
        public RelayCommand<string> GetImage { get; private set; }
        public RelayCommand<object> SelectCategory { get; private set; }
        public RelayCommand<object> SelectPrice { get; private set; }
        public RelayCommand CreateItem { get; private set; }

    }
}

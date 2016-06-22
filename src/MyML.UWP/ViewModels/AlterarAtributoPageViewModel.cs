using GalaSoft.MvvmLight.Command;
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
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;

namespace MyML.UWP.ViewModels
{
    public class AlterarAtributoPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public AlterarAtributoPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;


            DoCommand = new RelayCommand(DoCommandExecute);
            GetImage = new RelayCommand<string>(GetImageExecute);

        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (parameter != null)
            {
                var values = parameter.ToString().Split(new[] { '|' });
                SourceInfo = values[1];

                //Pega os detalhes do item
                var item = await _mercadoLivreService.GetItemDetails(values[0], new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "title,price,id,available_quantity,pictures") });
                if (item != null)
                {
                    foreach (var picture in item.pictures)
                    {
                        Pictures.Add(new ProductImage()
                        {
                            Source = new BitmapImage(new Uri(picture.url))
                        });
                    }
                   

                    Title = item.title;
                    Price = item.price ?? 0;
                    Quantity = item.available_quantity ?? 0;
                    ItemId = item.id;
                    var descriptionInfo = await _mercadoLivreService.GetProductDescrition(values[0]);
                    if (descriptionInfo != null)
                    {
                        Description = descriptionInfo.text;
                    }
                }
            }

            RaisePropertyChanged(() => Pictures);
            RaisePropertyChanged(() => AvailablePhotos);
        }


        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Pictures.Clear();
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
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
                    Pictures.Add(new ProductImage() { Source = bitmapImage, LocalPath = file.Path });
                    RaisePropertyChanged(() => AvailablePhotos);
                }
            }
        }

        private async void GetImageExecute(string source)
        {
            if (Pictures.Count >= 5)
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

            //AvailablePhotos = MaxImages - ProductInfo.Images.Count;
        }

        private async void DoCommandExecute()
        {
            try
            {
                if (SourceInfo == "title")
                {
                    Views.Shell.SetBusy(true, "Alterando atributo");
                    if (await _mercadoLivreService.ChangeProductAttributes(ItemId, new { title = Title }))
                        NavigationService.GoBack();
                }
                else if (SourceInfo == "price")
                {
                    Views.Shell.SetBusy(true, "Alterando atributo");
                    if (await _mercadoLivreService.ChangeProductAttributes(ItemId, new { price = Price }))
                        NavigationService.GoBack();
                }
                else if (SourceInfo == "quantity")
                {
                    Views.Shell.SetBusy(true, "Alterando atributo");
                    if (await _mercadoLivreService.ChangeProductAttributes(ItemId, new { available_quantity = Quantity }))
                        NavigationService.GoBack();
                }
                else if (SourceInfo == "description")
                {
                    Views.Shell.SetBusy(true, "Alterando descrição");
                    if (await _mercadoLivreService.ChangeProductDescription(ItemId, _description ))
                        NavigationService.GoBack();
                }
                else if (SourceInfo == "images")
                {
                    Views.Shell.SetBusy(true, "Enviando as imagens");

                    //Envia as imagens
                    foreach (var item in Pictures)
                    {
                        var imagemInfo = await _mercadoLivreService.UploadProductImage(item);
                        if (imagemInfo != null)
                        {
                            await _mercadoLivreService.AddPicture(imagemInfo.id, ItemId);
                        }
                    }
                    NavigationService.GoBack();
                }
            }
            finally
            {
                Views.Shell.SetBusy(false);      
            }
        }

        public string ItemId { get; set; }

        private string _sourceInfo;
        public string SourceInfo
        {
            get { return _sourceInfo; }
            set { Set(() => SourceInfo, ref _sourceInfo, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { Set(() => Description, ref _description, value); }
        }

        private double? _price;
        public double? Price
        {
            get { return _price; }
            set { Set(() => Price, ref _price, value); }
        }

        private int? _quantity;
        public int? Quantity
        {
            get { return _quantity; }
            set { Set(() => Quantity, ref _quantity, value); }
        }

        private ObservableCollection<ProductImage> _pictures = new ObservableCollection<ProductImage>();
        public ObservableCollection<ProductImage> Pictures
        {
            get { return _pictures; }
            set { Set(() => Pictures, ref _pictures, value); }
        }

        public RelayCommand DoCommand { get; set; }
        public RelayCommand<string> GetImage { get; private set; }


        public int AvailablePhotos => MaxImages - Pictures.Count;

        public int MaxImages { get; set; } = 5;
    }
}

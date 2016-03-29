using GalaSoft.MvvmLight.Command;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Navigation;

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
 
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(parameter != null)
            {
                var values = parameter.ToString().Split(new[] { '|' });
                SourceInfo = values[1];

                //Pega os detalhes do item
                var item = await _mercadoLivreService.GetItemDetails(values[0], new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "title,price,id,available_quantity") });
                if(item != null)
                {
                    Title = item.title;
                    Price = item.price ?? 0;
                    Quantity = item.available_quantity ?? 0;
                    ItemId = item.id;    
                }
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async void DoCommandExecute()
        {
            if(SourceInfo == "title")
            {
                if (await _mercadoLivreService.ChangeProductAttributes(ItemId, new { title = Title }))
                    NavigationService.GoBack();
            }
            else if(SourceInfo == "price")
            {
                if (await _mercadoLivreService.ChangeProductAttributes(ItemId, new { price = Price }))
                    NavigationService.GoBack();
            }
            else if (SourceInfo == "quantity")
            {
                if (await _mercadoLivreService.ChangeProductAttributes(ItemId, new { available_quantity = Quantity }))
                    NavigationService.GoBack();
            }
        }

        public string ItemId { get; set; }

        private string _SourceInfo;
        public string SourceInfo
        {
            get { return _SourceInfo; }
            set { Set(() => SourceInfo, ref _SourceInfo, value); }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { Set(() => Title, ref _Title, value); }
        }

        private double? _Price;
        public double? Price
        {
            get { return _Price; }
            set { Set(() => Price, ref _Price, value); }
        }

        private int? _Quantity;
        public int? Quantity
        {
            get { return _Quantity; }
            set { Set(() => Quantity, ref _Quantity, value); }
        }

        public RelayCommand DoCommand { get; set; }
    }
}

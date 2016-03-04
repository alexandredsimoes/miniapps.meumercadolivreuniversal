using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace MyML.UWP.ViewModels
{
    public class ComprarItemPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public ComprarItemPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;            
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(mode != NavigationMode.Back)
            {
                if (parameter == null) return;
                ItemId = parameter.ToString();

                await LoadItemDetails(ItemId);
            }
            
        }

        private async Task LoadItemDetails(string itemId)
        {
            if (!_dataService.IsAuthenticated())
            {
                await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                NavigationService.Navigate(typeof(LoginPage), ItemId, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return;
            }

            try
            {
                Shell.SetBusy(true, "Carregando informações");
                var item = await _mercadoLivreService.GetItemDetails(itemId);

                if (item != null)
                {
                    SelectedItem = item;
                    for (int i = 1; i <= item.available_quantity; i++)
                    {
                        _Quantities.Add(i);
                    }
                    RaisePropertyChanged(() => Quantities);
                }
            }
            finally
            {
                Shell.SetBusy(false);
            }

        }

        public string ItemId { get; set; }

        private Item _SelectedItem;
        public Item SelectedItem
        {
            get { return _SelectedItem; }
            set { Set(() => SelectedItem, ref _SelectedItem, value); }
        }

        private IList<int> _Quantities = new List<int>();
        public IList<int> Quantities
        {
            get { return _Quantities; }
            set { Set(() => Quantities, ref _Quantities, value); }
        }

    }
}

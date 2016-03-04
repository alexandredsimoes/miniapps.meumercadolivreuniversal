using GalaSoft.MvvmLight.Command;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using MyML.UWP.Views.Secure;
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
    public class AnunciosPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public AnunciosPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;


            Refresh = new RelayCommand(async () =>
            {
                if (!_dataService.IsAuthenticated())
                {
                    await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                    NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                    return;
                }

                await LoadAdverts("active");
                await LoadAdverts("paused");
                await LoadAdverts("closed");
            });
            SelectItem = new RelayCommand<object>((obj) =>
            {
                var item = obj as Item;
                if (item == null) return;
                NavigationService.Navigate(typeof(AnunciosDetalhePage), item.id);
            });

        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (!_dataService.IsAuthenticated())
            {
                await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return;
            }

            if (mode != NavigationMode.Back)
            {
                await LoadAdverts("active");
                await LoadAdverts("paused");
                await LoadAdverts("closed");
            }
        }
        private async Task LoadAdverts(string status)
        {            
            try
            {
                Views.Shell.SetBusy(true, "Carregando informações");
                var items = await _mercadoLivreService.ListMyItems(0, 10,
                    new KeyValuePair<string, object>[]
                    {
                        new KeyValuePair<string, object>("status",status)
                    });

                if (items != null)
                {
                    items.ListTypes = await _mercadoLivreService.ListTypes(Consts.ML_ID_BRASIL);

                    if (items.results_graph == null)
                        items.results_graph = new List<Item>();

                    foreach (var item in items.results)
                    {
                        var product = await _mercadoLivreService.GetItemDetails(item, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "id,title,price,thumbnail,stop_time,available_quantity") });
                        if (product != null)
                        {
                            if (items.ListTypes != null)
                                product.ListType = items.ListTypes.FirstOrDefault(c => c.id == product.listing_type_id);

                            items.results_graph.Add(product);
                        }
                    }
                    if (status == "active")
                        Items = items;
                    if (status == "paused")
                        ItemsPaused = items;
                    if (status == "closed")
                        ItemsClosed = items;
                }
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        public RelayCommand Refresh { get; private set; }
        public RelayCommand<object> SelectItem { get; private set; }


        private MLMyItemsSearchResult _Items;
        public MLMyItemsSearchResult Items
        {
            get { return _Items; }
            set { Set(() => Items, ref _Items, value); }
        }

        private MLMyItemsSearchResult _ItemsPaused;
        public MLMyItemsSearchResult ItemsPaused
        {
            get { return _ItemsPaused; }
            set { Set(() => ItemsPaused, ref _ItemsPaused, value); }
        }

        private MLMyItemsSearchResult _ItemsClosed;
        public MLMyItemsSearchResult ItemsClosed
        {
            get { return _ItemsClosed; }
            set { Set(() => ItemsClosed, ref _ItemsClosed, value); }
        }

    }
}

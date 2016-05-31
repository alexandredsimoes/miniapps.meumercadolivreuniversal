using GalaSoft.MvvmLight.Command;
using MyML.UWP.Adapters;
using MyML.UWP.Adapters.Search;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using MyML.UWP.Views.Secure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private CancellationTokenSource cts = new CancellationTokenSource(); 
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

                await LoadAdverts();
            });
            SelectItem = new RelayCommand<object>((obj) =>
            {
                var item = obj as Item;
                if (item == null) return;
                NavigationService.Navigate(typeof(AnunciosDetalhePage), item.id);
            });

        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (!_dataService.IsAuthenticated())
            {
                await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return;
            }

            if (mode != NavigationMode.Back && CacheHelper.IsExpired(nameof(AnunciosPageViewModel)))
            {
                await LoadAdverts();
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }
        private Task LoadAdverts()
        {            
            try
            {

                Items = new IncrementalSearchSource<AdvertsDataSource, Item>(0, 10, "active", true);
                ItemsPaused = new IncrementalSearchSource<AdvertsDataSource, Item>(0, 10, "paused", true);
                ItemsClosed = new IncrementalSearchSource<AdvertsDataSource, Item>(0, 10, "closed", true);

                Items.LoadMoreItemsStarted += () => { Shell.SetBusy(true); };
                ItemsPaused.LoadMoreItemsStarted += () => { Shell.SetBusy(true); };
                ItemsClosed.LoadMoreItemsStarted += () => { Shell.SetBusy(true); };

                Items.LoadMoreItemsCompleted += (paging) => { Shell.SetBusy(false); HasActiveItems = Items?.Count > 0; };
                ItemsPaused.LoadMoreItemsCompleted += (paging) => { Shell.SetBusy(false); HasPausedItems = ItemsPaused?.Count > 0; };
                ItemsClosed.LoadMoreItemsCompleted += (paging) => { Shell.SetBusy(false); HasClosedItems = ItemsClosed?.Count > 0; };

                CacheHelper.AddCache(nameof(AnunciosPageViewModel)); //Atualiza o cache da página            
                return Task.CompletedTask;               
            }
            finally
            {
                //Views.Shell.SetBusy(false);
            }
        }

        private bool _HasActiveItems;
        public bool HasActiveItems
        {
            get { return _HasActiveItems; }
            set { Set(() => HasActiveItems, ref _HasActiveItems, value); }
        }

        private bool _HasPausedItems;
        public bool HasPausedItems
        {
            get { return _HasPausedItems; }
            set { Set(() => HasPausedItems, ref _HasPausedItems, value); }
        }


        private bool _HasClosedItems;
        public bool HasClosedItems
        {
            get { return _HasClosedItems; }
            set { Set(() => HasClosedItems, ref _HasClosedItems, value); }
        }

        private IncrementalSearchSource<AdvertsDataSource, Item> _Items;
        public IncrementalSearchSource<AdvertsDataSource, Item> Items
        {
            get { return _Items; }
            set { Set(() => Items, ref _Items, value); }
        }

        private IncrementalSearchSource<AdvertsDataSource, Item> _ItemsPaused;
        public IncrementalSearchSource<AdvertsDataSource, Item> ItemsPaused
        {
            get { return _ItemsPaused; }
            set { Set(() => ItemsPaused, ref _ItemsPaused, value); }
        }

        private IncrementalSearchSource<AdvertsDataSource, Item> _ItemsClosed;
        public IncrementalSearchSource<AdvertsDataSource, Item> ItemsClosed
        {
            get { return _ItemsClosed; }
            set { Set(() => ItemsClosed, ref _ItemsClosed, value); }
        }

        public RelayCommand Refresh { get; private set; }
        public RelayCommand<object> SelectItem { get; private set; }       
    }
}

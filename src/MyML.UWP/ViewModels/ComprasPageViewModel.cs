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
using MyML.UWP.Models.Mercadolivre;
using Windows.UI.Popups;
using MyML.UWP.Adapters;
using MyML.UWP.Adapters.Search;
using MyML.UWP.Views;
using MyML.UWP.Views.Secure;

namespace MyML.UWP.ViewModels
{
    public class ComprasPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public ComprasPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;


            Refresh = new RelayCommand(LoadShopping);
            SelectOrder = new RelayCommand<object>((obj) =>
            {
                var item = obj as MLOrderInfo;
                if (item == null) return;
                NavigationService.Navigate(typeof(CompraDetalhePage), item.id);
            });

        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (!_dataService.IsAuthenticated())
            {
                //await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                //    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                Orders?.Clear();
                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return Task.CompletedTask;
            }

            if (mode != NavigationMode.Back)
                LoadShopping();

            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private void LoadShopping()
        {
            HasOrders = false;
            //Orders = await _mercadoLivreService.ListMyOrders(0, 0, new KeyValuePair<string, object>[] { });
            Orders = new IncrementalSearchSource<MyOrdersDataSource, MLOrderInfo>(0, 15, null, true);

            Orders.LoadMoreItemsStarted += () =>
            {
                Views.Shell.SetBusy(true, "Carregando informações");
                HasOrders = true;
            };

            Orders.LoadMoreItemsCompleted += (paging) =>
            {
                Views.Shell.SetBusy(false);
            };


        }

        public RelayCommand Refresh { get; private set; }
        public RelayCommand<object> SelectOrder { get; private set; }

        private IncrementalSearchSource<MyOrdersDataSource, MLOrderInfo> _orders;
        public IncrementalSearchSource<MyOrdersDataSource, MLOrderInfo> Orders
        {
            get { return _orders; }
            set { Set(() => Orders, ref _orders, value); }
        }

        /*private MLOrder _Orders;
        public MLOrder Orders
        {
            get { return _Orders; }
            set { Set(() => Orders, ref _Orders, value); }
        }*/

        private bool _hasOrders;

        public bool HasOrders
        {
            get { return _hasOrders; }
            set { Set(() => HasOrders, ref _hasOrders, value); }
        }

    }
}

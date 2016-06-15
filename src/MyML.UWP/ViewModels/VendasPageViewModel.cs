using GalaSoft.MvvmLight.Command;
using MyML.UWP.Adapters;
using MyML.UWP.Adapters.Search;
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
using GalaSoft.MvvmLight.Views;
using MyML.UWP.Models;

namespace MyML.UWP.ViewModels
{
    public class VendasPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;

        public VendasPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;


            Refresh = new RelayCommand(async () => await LoadShopping());
            SelectOrder = new RelayCommand<object>((obj) =>
            {
                var item = obj as MLOrderInfo;
                if (item == null) return;
                NavigationService.Navigate(typeof(VendaDetalhePage), item.id);
            });

            ShowAchivedOrders = new RelayCommand(() =>
            {
                NavigationService.Navigate(typeof(VendasArquivadasPage));
            });

        }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (mode != NavigationMode.Back)
                await LoadShopping();
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task LoadShopping()
        {
            if (!_dataService.IsAuthenticated())
            {
                await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return;
            }

            try
            {
                Orders = new IncrementalSearchSource<OrdersDataSource, MLOrderInfo>(0, 15, null, true);

                OrdersClosed = new IncrementalSearchSource<OrdersDataSource, MLOrderInfo>(0, 15, null, false);


                Orders.LoadMoreItemsStarted += () =>
                {
                    Views.Shell.SetBusy(true, "Carregando informações");
                };

                Orders.LoadMoreItemsCompleted += (paging) =>
                {
                    Views.Shell.SetBusy(false);
                };

                OrdersClosed.LoadMoreItemsStarted += () =>
                {
                    Views.Shell.SetBusy(true, "Carregando informações");
                };

                OrdersClosed.LoadMoreItemsCompleted += (paging) =>
                {
                    Views.Shell.SetBusy(false);
                };
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        public RelayCommand Refresh { get; private set; }
        public RelayCommand<object> SelectOrder { get; private set; }
        public RelayCommand ShowAchivedOrders { get; private set; }

        private IncrementalSearchSource<OrdersDataSource, MLOrderInfo> _Orders;
        public IncrementalSearchSource<OrdersDataSource, MLOrderInfo> Orders
        {
            get { return _Orders; }
            set { Set(() => Orders, ref _Orders, value); }
        }

        private IncrementalSearchSource<OrdersDataSource, MLOrderInfo> _OrdersClosed;
        public IncrementalSearchSource<OrdersDataSource, MLOrderInfo> OrdersClosed
        {
            get { return _OrdersClosed; }
            set { Set(() => OrdersClosed, ref _OrdersClosed, value); }
        }

        public bool ArchivedMode { get; set; } = false;
    }
}

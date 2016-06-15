using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.Adapters;
using MyML.UWP.Adapters.Search;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using Template10.Mvvm;

namespace MyML.UWP.ViewModels
{
    public class VendasArquivadasPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreServices;
        private readonly IDataService _dataService;
        private readonly ResourceLoader _resourceLoader;

        public VendasArquivadasPageViewModel(IMercadoLivreService mercadoLivreServices, IDataService dataService,
            ResourceLoader resourceLoader)
        {
            _mercadoLivreServices = mercadoLivreServices;
            _dataService = dataService;
            _resourceLoader = resourceLoader;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await LoadShopping();
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
                Orders = new IncrementalSearchSource<OrdersArchivedDataSource, MLOrderInfo>(0, 15, null, true);                
                Orders.LoadMoreItemsStarted += () =>
                {
                    Views.Shell.SetBusy(true, "Carregando informações");
                };

                Orders.LoadMoreItemsCompleted += (paging) =>
                {
                    Views.Shell.SetBusy(false);
                };               
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }


        private IncrementalSearchSource<OrdersArchivedDataSource, MLOrderInfo> _orders;
        public IncrementalSearchSource<OrdersArchivedDataSource, MLOrderInfo> Orders
        {
            get { return _orders; }
            set { Set(() => Orders, ref _orders, value); }
        }
    }
}

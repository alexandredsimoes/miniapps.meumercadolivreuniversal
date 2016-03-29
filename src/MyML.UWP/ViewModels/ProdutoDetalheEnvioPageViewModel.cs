using GalaSoft.MvvmLight.Command;
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
    public class ProdutoDetalheEnvioPageViewModel : ViewModelBase
    {        
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly IDataService _dataService;
        private readonly ResourceLoader _resourceLoader;

        public ProdutoDetalheEnvioPageViewModel(IMercadoLivreService mercadoLivreService, IDataService dataService, ResourceLoader resourceLoader)
        {
            _mercadoLivreService = mercadoLivreService;
            _dataService = dataService;
            _resourceLoader = resourceLoader;

            CalculateShipping = new RelayCommand(CalculateShippingExecute);
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (parameter == null) return Task.CompletedTask;

            ProductId = parameter.ToString();

            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            ShippingInfo = null;
            Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async void CalculateShippingExecute()
        {
            try
            {
                Shell.SetBusy(true, "aguarde...");
                if (string.IsNullOrWhiteSpace(ZipCode))
                {
                    await new MessageDialog(_resourceLoader.GetString("ProductSearchResultPageViewModelShippingZipCodeEmpty"),
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    return;
                }

                var result = await _mercadoLivreService.GetShippingCost(ProductId, ZipCode);
                if (result == null || result.destination == null)
                {
                    await new MessageDialog(_resourceLoader.GetString("ProductSearchResultPageViewModelErrorLoadingShippingInfo"),
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                    ShippingInfo = null;
                    return;
                }
                ShippingInfo = result;
            }
            finally
            {
                Shell.SetBusy(false);
            }            
        }

        private string _ZipCode;
        public string ZipCode
        {
            get { return _ZipCode; }
            set { Set(() => ZipCode, ref _ZipCode, value); }
        }

        private ShippingCost _ShippingInfo;
        public ShippingCost ShippingInfo
        {
            get { return _ShippingInfo; }
            set { Set(() => ShippingInfo, ref _ShippingInfo, value); }
        }

        public string ProductId { get; set; }


        public RelayCommand CalculateShipping { get; private set; }
    }
}

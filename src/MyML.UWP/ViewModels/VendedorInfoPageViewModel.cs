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

namespace MyML.UWP.ViewModels
{
    public class VendedorInfoPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;

        

        public VendedorInfoPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;

            if(GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
               
            }
            
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(parameter != null)
            {
                //Carrega os dados do vendedor
                SellerInfo = await _mercadoLivreService.GetUserInfo(parameter.ToString(), null);
            }            
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private MLUserInfoSearchResult _SellerInfo;
        public MLUserInfoSearchResult SellerInfo
        {
            get { return _SellerInfo; }
            set { Set(() => SellerInfo, ref _SellerInfo, value); }
        }        
    }
}

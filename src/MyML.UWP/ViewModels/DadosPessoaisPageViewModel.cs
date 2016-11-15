using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using Template10.Mvvm;

namespace MyML.UWP.ViewModels
{
    public class DadosPessoaisPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly IDataService _dataService;
        private MLUserInfoSearchResult _me;

        public DadosPessoaisPageViewModel(IMercadoLivreService mercadoLivreService, IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _dataService = dataService;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (!_dataService.IsAuthenticated())
            {
                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                await Task.CompletedTask;
                return;
            }
            Me = await _mercadoLivreService.GetUserProfile();                        

        }

        public MLUserInfoSearchResult Me
        {
            get { return _me; }
            set { Set(ref _me, value); }
        }
    }
}
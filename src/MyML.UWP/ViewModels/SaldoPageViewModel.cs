using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.Services;
using MyML.UWP.Views;
using Template10.Mvvm;

namespace MyML.UWP.ViewModels
{
    public class SaldoPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreServices;
        private readonly IDataService _dataService;
        public SaldoPageViewModel(IMercadoLivreService mercadoLivreServices, IDataService dataService)
        {
            _mercadoLivreServices = mercadoLivreServices;
            _dataService = dataService;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (!_dataService.IsAuthenticated())
            {
                Balance = 0;
                BalanceWithDraw = 0;
                UnavailableBalance = 0;
                await NavigationService.NavigateAsync(typeof(LoginPage));
                return;
            }
            await LoadSummary();
        }

        private async Task LoadSummary()
        {
            //Pega os dados do balanço
            var accountBalance = await _mercadoLivreServices.GetUserAccountBalance();

            if (accountBalance != null)
            {
                BalanceWithDraw = accountBalance.available_balance;
                Balance = accountBalance.total_amount;
                UnavailableBalance = accountBalance.unavailable_balance;
            }
        }

        private double? _unavailableBalance = 0;
        public double? UnavailableBalance
        {
            get { return _unavailableBalance; }
            set { Set(() => UnavailableBalance, ref _unavailableBalance, value); }
        }


        private double? _balance = 0;
        public double? Balance
        {
            get { return _balance; }
            set { Set(() => Balance, ref _balance, value); }
        }

        private double? _balanceWithDraw = 0;
        public double? BalanceWithDraw
        {
            get { return _balanceWithDraw; }
            set { Set(() => BalanceWithDraw, ref _balanceWithDraw, value); }
        }
    }
}

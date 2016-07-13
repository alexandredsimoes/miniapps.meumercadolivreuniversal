using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using MyML.UWP.Adapters;
using MyML.UWP.Adapters.Search;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace MyML.UWP.ViewModels
{
    public class ProdutoUsuarioPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        public ProdutoUsuarioPageViewModel(IMercadoLivreService mercadoLivreService)
        {
            _mercadoLivreService = mercadoLivreService;

            SelecionarProduto = new RelayCommand<object>(o =>
            {
                var eArgs = o as RoutedEventArgs;
                var element = eArgs?.OriginalSource as FrameworkElement;

                var item = element?.DataContext as Item;

                if (item == null) return;

                NavigationService.Navigate(typeof(ProdutoDetalhePage), item.id);
            });
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (parameter != null && mode == NavigationMode.New)
            {                
                await LoadItems(parameter);
            }            
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if (args.NavigationMode == NavigationMode.Back && !args.Suspending)
            {
                //Limpa a bagunça toda
                if (Items != null)
                {
                    Items.Clear();
                    Items.LoadMoreItemsCompleted -= Items_LoadMoreItemsCompleted;
                    Items.LoadMoreItemsStarted -= Items_LoadMoreItemsStarted;
                    Items.FiltersAvailable -= Items_FiltersAvailable;
                    Items = null;
                }
            }

            Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task LoadItems(object parameter)
        {

            //Tenta pegar os dados do vendedor
            var user =
                await
                    _mercadoLivreService.GetUserInfo(parameter.ToString(), new KeyValuePair<string, object>("attributes", "nickname"));
            if (user != null)
            {
                Title = user.nickname;
            }
            Items = new IncrementalSearchSource<SearchProductUserDataSource, Item>(0, 15, parameter.ToString(), false);

            Items.FiltersAvailable += Items_FiltersAvailable;
            Items.LoadMoreItemsCompleted += Items_LoadMoreItemsCompleted;
            Items.LoadMoreItemsStarted += Items_LoadMoreItemsStarted;            
        }

        private void Items_LoadMoreItemsStarted()
        {
            Shell.SetBusy(true);
        }

        private void Items_LoadMoreItemsCompleted(Paging obj)
        {
            Shell.SetBusy(false);
        }

        private void Items_FiltersAvailable(IList<AvailableFilter> arg1, IList<AvailableSort> arg2)
        {
            
        }

        private IncrementalSearchSource<SearchProductUserDataSource, Item> _items;
        public IncrementalSearchSource<SearchProductUserDataSource, Item> Items
        {
            get { return _items; }
            set { Set(() => Items, ref _items, value); }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }
        public RelayCommand<object> SelecionarProduto { get; private set; }
    }
}

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
    public class FavoritesPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;

        public FavoritesPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;


            Refresh = new RelayCommand(async () =>
            {
                await LoadItems();
            });
            SelectItem = new RelayCommand<object>((obj) =>
            {
                var item = obj as MLBookmarkItem;
                if (item == null) return;
                NavigationService.Navigate(typeof(ProdutoDetalhePage), item.item_id);
            });

            RemoveBookmark = new RelayCommand<object>(async (obj) =>
            {
                var item = obj as MLBookmarkItem;
                if (item == null) return;

                if (await _mercadoLivreService.RemoveBookmarkItem(item.item_id))
                    await LoadItems();
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
            if (mode != NavigationMode.Back && CacheHelper.IsExpired(nameof(FavoritesPageViewModel)))
                await LoadItems();
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task LoadItems()
        {
            try
            {
                Views.Shell.SetBusy(true, "Carregando informações");
                var items = await _mercadoLivreService.GetBookmarkItems();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        item.ItemInfo = await _mercadoLivreService.GetItemDetails(item.item_id, new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("attributes", "id,thumbnail,price,title")
                        });
                    }
                    Bookmarks = items;
                    HasItems = Bookmarks.Count > 0;

                    CacheHelper.AddCache(nameof(FavoritesPageViewModel)); //Atualiza o cache da página
                }
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }

        }

        private bool _HasItems = false;

        public bool HasItems
        {
            get { return _HasItems; }
            set { Set(() => HasItems, ref _HasItems, value); }
        }

        private IList<MLBookmarkItem> _Bookmarks = new List<MLBookmarkItem>();
        public IList<MLBookmarkItem> Bookmarks
        {
            get { return _Bookmarks; }
            set { Set(() => Bookmarks, ref _Bookmarks, value); }
        }


        public RelayCommand Refresh { get; set; }
        public RelayCommand<object> SelectItem { get; set; }
        public RelayCommand<object> RemoveBookmark { get; set; }

    }
}

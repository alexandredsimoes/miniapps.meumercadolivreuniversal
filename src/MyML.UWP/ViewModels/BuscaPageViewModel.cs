﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MyML.UWP.Adapters;
using MyML.UWP.Adapters.Search;
using MyML.UWP.Aggregattor;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using Microsoft.HockeyApp;
using MyML.UWP.Services.SettingsServices;

namespace MyML.UWP.ViewModels
{
    public class BuscaPageViewModel : ViewModelBase
    {
        
        private readonly IMercadoLivreService _mercadoLivreService;
        public IList<AvailableFilter> SelectedFilters { get; set; } = new List<AvailableFilter>();

        public BuscaPageViewModel(IMercadoLivreService mercadoLivreService)
        {
            _mercadoLivreService = mercadoLivreService;

            Messenger.Default.Register<MessengerFilterResult>(this, GetFilterResult);
            Buscar = new RelayCommand<string>((s) =>
            {
                BuscaExecute(new[] { s, "search" });
            });
            SelecionarProduto = new RelayCommand<object>(SelecionarProdutoExecute);

            OpenFilter = new RelayCommand(() =>
            {
                if (Items == null || Items.Count == 0) return;
                NavigationService.Navigate(typeof(BuscaFilterPage));
                Messenger.Default.Send<MessengerFilterDetails>(new MessengerFilterDetails()
                {
                    Filters = this.Filters,
                    Sorts = this.Sorts,

                });
            });

            OpenSort = new RelayCommand(() =>
            {
                if (Items == null || Items.Count == 0) return;
                NavigationService.Navigate(typeof(OrdernarBuscaPage));
                Messenger.Default.Send<MessengerFilterDetails>(new MessengerFilterDetails()
                {
                    Filters = this.Filters,
                    Sorts = this.Sorts,

                });
            });         
        }

        private void GetFilterResult(MessengerFilterResult obj)
        {
            if (obj == null) return;

            BuscaExecute(new[] { Searchterm + "&" + obj.Result, SearchType });
        }

        public  override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            //if (state.ContainsKey("Searchterm"))
            //    Searchterm = (string)state["Searchterm"];
            //if(state.ContainsKey("SCROLL_POSITION"))
            //{
            //    ScrollPosition = (int)state["SCROLL_POSITION"];
            //}

            //await new Windows.UI.Popups.MessageDialog(parameter.ToString(), "param").ShowAsync();
            if (mode == NavigationMode.Back)
            {
                //Items.GetEnumerator().
            }

            if (parameter != null && mode == NavigationMode.New)
            {
                var search = parameter.ToString().Split('|');
                if (search.Length == 1)
                {
                    Searchterm = search[0];
                    BuscaExecute(new[] { Searchterm, "search" });
                }
                else
                {
                    Searchterm = search[0];
                    SearchType = search[1];
                    BuscaExecute(new[] { Searchterm, SearchType });
                }
            }
            return Task.CompletedTask;
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

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            //if (pageState.ContainsKey("SCROLL_POSITION"))
            //    pageState["SCROLL_POSITION"] = ScrollPosition;
            //else
            //    pageState.Add("SCROLL_POSITION", ScrollPosition);

            //if (!pageState.ContainsKey("Searchterm"))
            //    pageState.Add("Searchterm", Searchterm);
            //else
            //    pageState["Searchterm"] = Searchterm;

            return Task.CompletedTask;
        }


        private void SelecionarProdutoExecute(object obj)
        {
            var eArgs = obj as RoutedEventArgs;
            var element = eArgs?.OriginalSource as FrameworkElement;
            
            var item = element?.DataContext as Item;

            if (item == null) return;

            NavigationService.Navigate(typeof(ProdutoDetalhePage), item.id);            
        }


        private async void BuscaExecute(string[] parametro)
        {
            if (parametro == null || parametro.Length != 2) return;

            Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();

            var query = parametro[0];
            ResultQuery = query;
            if (parametro[1] == "category")
            {
                //Obtem os detalhes da categorias
                var category = await _mercadoLivreService.GetCategoryDetail(query);

                if (category != null)
                {
                    //ResultQuery = String.Format("{0} - {1} resultados", category.name, category.total_items_in_this_category);
                }
            }
            else if (parametro[1] == "search")
            {
                //ResultQuery = String.Format("Busca por " + query);
            }

            var family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            var pageSize = 25;

            if (family.Equals("Windows.Desktop") || family.Equals("Windows.Xbox"))
                pageSize = 50;

            var settings = SettingsService.Instance;
            Items = new IncrementalSearchSource<SearchDataSource, Item>(0, pageSize, query, parametro[1] != "category",
                settings.UseHighQualityImages ?? false);

            Items.FiltersAvailable += Items_FiltersAvailable;
            Items.LoadMoreItemsCompleted += Items_LoadMoreItemsCompleted;
            Items.LoadMoreItemsStarted += Items_LoadMoreItemsStarted;            

            HockeyClient.Current.TrackEvent("BuscaPageViewModel.BuscarExecute()");
            //_telemetryClient.TrackEvent("BuscaPageViewModel", new Dictionary<string, string>() { { "Pesquisa", query } });
            
        }

        private void Items_FiltersAvailable(IList<AvailableFilter> listFilters, IList<AvailableSort> listSorts)
        {
            Filters = listFilters;
            Sorts = listSorts;
        }

        private void Items_LoadMoreItemsStarted()
        {
            Views.Shell.SetBusy(true, "Carregando informações");
        }

        private void Items_LoadMoreItemsCompleted(Paging paging)
        {
            if (paging != null)
                ScrollPosition = paging.offset;

            Views.Shell.SetBusy(false);
        }

        private IncrementalSearchSource<SearchDataSource, Item> _Items;
        public IncrementalSearchSource<SearchDataSource, Item> Items
        {
            get { return _Items; }
            set { Set(() => Items, ref _Items, value); }
        }

        //private MercadoLivreIncrementalLoadingClass<Item> _Items;
        //public MercadoLivreIncrementalLoadingClass<Item> Items
        //{
        //    get { return _Items; }
        //    set { Set(() => Items, ref _Items, value); }
        //}

        private IList<AvailableFilter> _Filters;
        public IList<AvailableFilter> Filters
        {
            get { return _Filters; }
            set { Set(() => Filters, ref _Filters, value); }
        }

        private IList<AvailableSort> _Sorts;
        public IList<AvailableSort> Sorts
        {
            get { return _Sorts; }
            set { Set(() => Sorts, ref _Sorts, value); }
        }

        private string _ResultQuery;
        public string ResultQuery
        {
            get { return _ResultQuery; }
            set { Set(() => ResultQuery, ref _ResultQuery, value); }
        }

        private int _ScrollPosition;
        public int ScrollPosition
        {
            get { return _ScrollPosition; }
            set { Set(() => ScrollPosition, ref _ScrollPosition, value); }
        }

        private string _Searchterm;

        public string Searchterm
        {
            get { return _Searchterm; }
            set { Set(() => Searchterm, ref _Searchterm, value); }
        }

        private string _SearchType;
        public string SearchType
        {
            get { return _SearchType; }
            set { Set(() => SearchType, ref _SearchType, value); }
        }

        public Item SelectedItem { get; set; }


        public RelayCommand<string> Buscar { get; private set; }
        public RelayCommand<object> SelecionarProduto { get; private set; }
        public RelayCommand OpenFilter { get; private set; }
        public MLMyItemsSearchResult Items2 { get; private set; }
        public RelayCommand OpenSort { get; private set; }        
    }
}

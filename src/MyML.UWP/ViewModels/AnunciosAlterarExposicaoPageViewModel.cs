using GalaSoft.MvvmLight.Command;
using MyML.UWP.AppStorage;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Navigation;

namespace MyML.UWP.ViewModels
{
    public class AnunciosAlterarExposicaoPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public AnunciosAlterarExposicaoPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;


            ChangeListType = new RelayCommand<object>(ChangeListTypeExecute);
            //DoCommand = new RelayCommand<string>(DoCommandExecute);
            //SelectItem = new RelayCommand<object>((obj) =>
            //{
            //    var item = obj as Item;
            //    if (item == null) return;
            //    NavigationService.Navigate(typeof(AnunciosDetalhePage), item.id);
            //});
        }

        private async void ChangeListTypeExecute(object obj)
        {
            var itemType = obj as MLListType;

            if (itemType == null) return;

            if (await _mercadoLivreService.ChangeItemListType(ItemId, itemType.id))
                NavigationService.GoBack();
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (mode != NavigationMode.Back)
            {
                if (parameter != null)
                {
                    ItemId = parameter.ToString();
                    await LoadTypes(ItemId);
                }
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task LoadTypes(string itemId)
        {
            try
            {
                Views.Shell.SetBusy(true, "Listando opções");
                var product = await _mercadoLivreService.GetItemDetails(itemId, new KeyValuePair<string, object>("attributes", "id,price,listing_type_id"));

                if (product != null)
                {
                    //Lista os upgrades disponiveis para o tipo
                    var upgrades = await _mercadoLivreService.GetAvailableUpgrades(itemId);
                    if (upgrades != null)
                    {
                        foreach (var item in upgrades)
                        {
                            var prices = await _mercadoLivreService.GetListingPrices(item.site_id, product.price ?? 0, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("listing_type_id", item.id) });
                            if (prices != null)
                                item.Price = prices.FirstOrDefault();
                        }
                        Upgrades = upgrades;
                    }                    
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("", ex);
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }


        }

        public string ItemId { get; set; }
        private IReadOnlyCollection<MLListType> _Upgrades;

        public IReadOnlyCollection<MLListType> Upgrades
        {
            get { return _Upgrades; }
            set { Set(() => Upgrades, ref _Upgrades, value); }
        }

        public RelayCommand<object> ChangeListType { get; private set; }

    }
}

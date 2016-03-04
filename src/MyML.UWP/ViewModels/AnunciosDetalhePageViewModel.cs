using GalaSoft.MvvmLight.Command;
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

namespace MyML.UWP.ViewModels
{
    public class AnunciosDetalhePageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public AnunciosDetalhePageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;


            DoCommand = new RelayCommand<string>(DoCommandExecute);
            ChangeAttribute = new RelayCommand<string>((item) =>
            {
                NavigationService.Navigate(typeof(AlterarAtributoPage), String.Concat(ItemId, "|", item));
            });
            //SelectItem = new RelayCommand<object>((obj) =>
            //{
            //    var item = obj as Item;
            //    if (item == null) return;
            //    NavigationService.Navigate(typeof(AnunciosDetalhePage), item.id);
            //});
        }

        private async void DoCommandExecute(string command)
        {
            try
            {
                MessageDialog dlg = null;
                Views.Shell.SetBusy(true, "Atualizando informações");
                switch (command)
                {
                    case "E":
                        NavigationService.Navigate(typeof(AnunciosAlterarExposicaoPage), ItemId);
                        break;
                    case "P":
                        if (await _mercadoLivreService.ChangeProductStatus(ItemId, MLProductStatus.mlpsPause))
                            NavigationService.GoBack();
                        break;
                    case "F":
                        DateTime data = DateTime.MinValue;
                        if (DateTime.TryParse(Item.stop_time, out data))
                        {
                            TimeSpan s = data.Subtract(DateTime.Now);

                            dlg = new MessageDialog(string.Format(_resourceLoader.GetString("AnunciosDetalhePageFinalizar"), s.Days),
                            _resourceLoader.GetString("AnunciosDetalhePageFinalizarTitle"));
                            dlg.Commands.Add(new UICommand(_resourceLoader.GetString("Finalize"), async (a) =>
                            {
                                if (await _mercadoLivreService.ChangeProductStatus(ItemId, MLProductStatus.mlpsClose))
                                    NavigationService.GoBack();
                            }));
                            dlg.Commands.Add(new UICommand(_resourceLoader.GetString("Cancel"), (a) => {/*Nada faz*/}));

                            await dlg.ShowAsync();
                        }
                        break;
                    case "R":
                        if (await _mercadoLivreService.ChangeProductStatus(ItemId, MLProductStatus.mlpsActive))
                            NavigationService.GoBack();
                        break;
                    case "V": //Ver anuncio
                        NavigationService.Navigate(typeof(ProdutoDetalhePage), ItemId);
                        break;
                    case "N": //Recadastrar
                        if (await _mercadoLivreService.RelistProduct(Item.id, Item.available_quantity ?? 0, Item.price ?? 0, Item.listing_type_id))
                            NavigationService.GoBack();
                        break;
                    case "X": //Eliminar
                        dlg = new MessageDialog(_resourceLoader.GetString("AnunciosDetalhePageRemover"),
                            _resourceLoader.GetString("ApplicationName"));
                        dlg.Commands.Add(new UICommand(_resourceLoader.GetString("Delete"), async (a) =>
                        {
                            if (await _mercadoLivreService.RemoveProduct(Item.id))
                                NavigationService.GoBack();
                        }));
                        dlg.Commands.Add(new UICommand(_resourceLoader.GetString("Cancel"), (a) => {/*Nada faz*/}));

                        await dlg.ShowAsync();

                        break;
                }
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }

        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (mode != NavigationMode.Back)
            {
                if (parameter == null) return;
                ItemId = parameter.ToString();
                await LoadItem(ItemId.ToString());
            }
        }

        private async Task LoadItem(string itemId)
        {
            try
            {
                Views.Shell.SetBusy(true, "Carregando as informações");
                var item = await _mercadoLivreService.GetItemDetails(itemId);

                if (item != null)
                {
                    //Pega a descrição do cabra
                    var description = await _mercadoLivreService.GetProductDescrition(itemId);
                    if (description != null)
                    {
                        if (!string.IsNullOrWhiteSpace(description.plain_text))
                            ItemDescription = description.plain_text;
                        else
                            ItemDescription = _resourceLoader.GetString("AnunciosDetalhePageDescriptionHtml");
                    }

                    //Pega as visitas do produto
                    var visits = await _mercadoLivreService.GetProductVisits(itemId, DateTime.Now.AddYears(-1), DateTime.Now);
                    if (visits != null)
                        ItemVisits = visits.total_visits;

                    //Pega as perguntas do item
                    var questions = await _mercadoLivreService.ListQuestionsByProduct(itemId, 0, 0, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "total") });
                    if (questions != null)
                        ItemQuestions = questions.total ?? 0;

                    Item = item;

                }
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }


        private Item _Item;
        public Item Item
        {
            get { return _Item; }
            set { Set(() => Item, ref _Item, value); }
        }

        private string _ItemDescription;
        public string ItemDescription
        {
            get { return _ItemDescription; }
            set { Set(() => ItemDescription, ref _ItemDescription, value); }
        }

        private int _ItemVisits;
        public int ItemVisits
        {
            get { return _ItemVisits; }
            set { Set(() => ItemVisits, ref _ItemVisits, value); }
        }

        private int _ItemQuestions;
        public int ItemQuestions
        {
            get { return _ItemQuestions; }
            set { Set(() => ItemQuestions, ref _ItemQuestions, value); }
        }

        public string ItemId { get; set; }

        public RelayCommand<string> DoCommand { get; set; }
        public RelayCommand<string> ChangeAttribute { get; set; }
    }
}

using GalaSoft.MvvmLight.Command;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Utils;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Template10.Services.NavigationService;
using System.Collections.ObjectModel;

namespace MyML.UWP.ViewModels
{
    public class PerguntasComprasPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public PerguntasComprasPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;

            SelectProduct = new RelayCommand<object>((o) =>
            {
                var question = o as QuestionGroup;
                if (question == null) return;

                NavigationService.Navigate(typeof(ProdutoDetalhePage), question.Produto.id);
            });

            MakeQuestion = new RelayCommand<object>((o) =>
            {                
                if (o == null) return;

                NavigationService.Navigate(typeof(PerguntarPage), o);
            });


            Refresh = new RelayCommand(async () => await LoadQuestions());
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(mode != NavigationMode.Back)
            {
                await LoadQuestions();
            }            
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            //if(args.NavigationMode == NavigationMode.Back)
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task LoadQuestions()
        {
            if (!_dataService.IsAuthenticated())
            {
                await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return;
            }

            Views.Shell.SetBusy(true, "Carregando informações");

            var questions = await _dataService.ListQuestions();

            try
            {
                Questions.Clear();
                foreach (var item in questions)
                {
                    //Tenta obter os detalhes da questao
                    var questionDetail = await _mercadoLivreService.GetQuestionDetails(item.id.ToString(), new KeyValuePair<string, object>[] { });
                    var productDetail = await _mercadoLivreService.GetItemDetails(item.item_id, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "id,title,price,thumbnail") });

                    if (questionDetail == null || productDetail == null)
                    {
                        await new MessageDialog(_resourceLoader.GetString("PerguntasComprasPageMsgErrorLoadQuestionDetail"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
                        break;
                    }

                    item.answer = questionDetail.answer;
                    item.date_created = questionDetail.date_created;
                    item.id = questionDetail.id;
                    item.seller_id = questionDetail.seller_id;
                    item.status = questionDetail.status;
                    item.text = questionDetail.text;
                    item.Item = productDetail;
                }

                Questions = questions
                    .GroupBy(c => new { id = c.Item.id, title = c.Item.title, price = c.Item.price, thumbnail = c.Item.thumbnail, available_quantity = c.Item.available_quantity })

                    .Select(x => new QuestionGroup()
                    {
                        Produto = new Item() { id = x.Key.id, title = x.Key.title, price = x.Key.price, thumbnail = x.Key.thumbnail, available_quantity = x.Key.available_quantity },
                        Perguntas = new ObservableCollection<ProductQuestionContent>(x.Select(q => new ProductQuestionContent()
                        {
                            answer = q.answer,
                            date_created = q.date_created,
                            id = q.id,
                            item_id = q.item_id,
                            seller_id = q.seller_id,
                            status = q.status,
                            text = q.text
                        }).ToList())
                    })
                    .ToList();

                RaisePropertyChanged(() => Questions);

            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        private string _ProductId;
        public string ProductId
        {
            get { return _ProductId; }
            set { Set(() => ProductId, ref _ProductId, value); }
        }


        private IList<QuestionGroup> _Questions = new List<QuestionGroup>();

        public IList<QuestionGroup> Questions
        {
            get { return _Questions; }
            set { Set(() => Questions, ref _Questions, value); }
        }

        public RelayCommand<object> SelectProduct { get; private set; }
        public RelayCommand<object> MakeQuestion { get; private set; }
        public RelayCommand Refresh { get; private set; }



        //public class QuestionInfo : ProductQuestionContent
        //{
        //    public Item ItemInfo { get; set; }
        //}
    }
}

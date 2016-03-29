using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MyML.UWP.Aggregattor;
using MyML.UWP.Models;
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
using Template10.Services.NavigationService;
using System.Collections.ObjectModel;

namespace MyML.UWP.ViewModels
{
    public class PerguntasVendasPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public PerguntasVendasPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
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

            SelectQuestion = new RelayCommand<object>((o) =>
            {
                var question = o as ProductQuestionContent;
                if (question == null) return;

                NavigationService.Navigate(typeof(PerguntasVendasDetalhesPage), question.id);
            });

            Refresh = new RelayCommand(async () => await LoadQuestions());
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (mode != NavigationMode.Back)
            {
                await LoadQuestions();
            }

            Messenger.Default.Register<MessengerDetails>(this, ProccessMessenger);
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if (args.Suspending || args.NavigationMode == NavigationMode.Back)
                Messenger.Default.Unregister<MessengerDetails>(this, ProccessMessenger);

            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async void ProccessMessenger(MessengerDetails obj)
        {
            string lastItem = null;
            foreach (var item in Questions)
            {
                if (item.Produto.id == obj.SubId.ToString())
                {
                    long id = 0;
                    if(long.TryParse(obj.Id.ToString(), out id))
                    {
                        await Task.Delay(2000); //Aguarda um pouco
                        var itemToRemove = item.Perguntas.FirstOrDefault(c => c.id == id);
                        if (itemToRemove != null)
                            item.Perguntas.Remove(itemToRemove);

                        if (item.Perguntas.Count == 0)
                            lastItem = item.Produto.id;
                    } 
                }
            }
            //if (lastItem != null)
            //{
            //    var item = Questions.FirstOrDefault(c => c.Produto.id == lastItem);
            //    if (item != null)
             //       Questions.Remove(item);
            //}                
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

            var questions = await _mercadoLivreService.ListQuestions(new KeyValuePair<string, object>[] {
                new KeyValuePair<string, object>("status","unanswered")
            });

            try
            {
                Questions.Clear();
                foreach (var item in questions.questions)
                {
                    //Tenta obter os detalhes da questao
                    var questionDetail = await _mercadoLivreService.GetQuestionDetails(item.id.ToString(), new KeyValuePair<string, object>[] { });
                    var productDetail = await _mercadoLivreService.GetItemDetails(item.item_id, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "id,title,price,thumbnail") });

                    if (questionDetail == null || productDetail == null)
                    {
                        await new MessageDialog(_resourceLoader.GetString("PerguntasComprasPageMsgErrorLoadQuestionDetail"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
                        break;
                    }

                    //item.answer = questionDetail.answer;
                    item.date_created = questionDetail.date_created;
                    item.id = questionDetail.id;
                    item.seller_id = questionDetail.seller_id;
                    item.status = questionDetail.status;
                    item.text = questionDetail.text;
                    item.ProductInfo = productDetail;
                }

                Questions = new ObservableCollection<QuestionGroup>(questions.questions
                    .GroupBy(c => new
                    {
                        id = c.ProductInfo.id,
                        title = c.ProductInfo.title,
                        price = c.ProductInfo.price,
                        thumbnail = c.ProductInfo.thumbnail,
                        available_quantity = c.ProductInfo.available_quantity
                    })

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
                    .ToList());

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


        private ObservableCollection<QuestionGroup> _Questions = new ObservableCollection<QuestionGroup>();

        public ObservableCollection<QuestionGroup> Questions
        {
            get { return _Questions; }
            set { Set(() => Questions, ref _Questions, value); }
        }

        public RelayCommand<object> SelectProduct { get; private set; }
        public RelayCommand<object> MakeQuestion { get; private set; }
        public RelayCommand<object> SelectQuestion { get; private set; }
        public RelayCommand Refresh { get; private set; }


    }
}

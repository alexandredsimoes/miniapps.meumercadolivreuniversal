using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MyML.UWP.Aggregattor;
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
    public class PerguntasVendasDetalhesPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public PerguntasVendasDetalhesPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;

            SelectProduct = new RelayCommand<object>((o) =>
            {
                var question = o as ProductQuestionContent;
                if (question == null) return;

                NavigationService.Navigate(typeof(ProdutoDetalhePage), question.item_id);
            });

            SendAnswer = new RelayCommand(SendAnswerExecute);

            //SelectQuestion = new RelayCommand<object>((o) =>
            //{
            //    var question = o as ProductQuestionContent;
            //    if (question == null) return;

            //    NavigationService.Navigate(typeof(PerguntasVendasDetalhesPage), question.id);
            //});

            //Refresh = new RelayCommand(async () => await LoadQuestions());
        }

        private async void SendAnswerExecute()
        {
           if(string.IsNullOrWhiteSpace(AnswerText))
            {
                await new MessageDialog(_resourceLoader.GetString("PerguntasVendasDetalhesPageMsgEmptyAnswer"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

            if (await _mercadoLivreService.AnswerQuestion(QuestionId, AnswerText))
            {                
                Messenger.Default.Send<MessengerDetails>(new MessengerDetails()
                {
                   Id = QuestionId,
                   SubId = ProductId,
                   Source = SourceDetail.sdSellAnswer
                });

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (mode != NavigationMode.Back)
            {
                if (parameter != null)
                    await LoadQuestions(parameter.ToString());
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task LoadQuestions(string questionId)
        {
            QuestionId = questionId;
            if (!_dataService.IsAuthenticated())
            {
                await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return;
            }
                                  
            try
            {
                Views.Shell.SetBusy(true, "Carregando informações");
                var question = await _mercadoLivreService.GetQuestionDetails(questionId);

                if(question != null)
                {
                    ProductId = question.item_id;
                    question.Item = await _mercadoLivreService.GetItemDetails(question.item_id,
                        new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("attributes","title,thumbnail,price,available_quantity")
                        });

                    QuestionInfo = question;
                }
                //foreach (var item in questions.questions)
                //{
                //    //Tenta obter os detalhes da questao
                //    var questionDetail = await _mercadoLivreService.GetQuestionDetails(item.id.ToString(), new KeyValuePair<string, object>[] { });
                //    var productDetail = await _mercadoLivreService.GetItemDetails(item.item_id, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("attributes", "id,title,price,thumbnail") });

                //    if (questionDetail == null || productDetail == null)
                //    {
                //        await new MessageDialog(_resourceLoader.GetString("PerguntasComprasPageMsgErrorLoadQuestionDetail"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
                //        break;
                //    }

                //    //item.answer = questionDetail.answer;
                //    item.date_created = questionDetail.date_created;
                //    item.id = questionDetail.id;
                //    item.seller_id = questionDetail.seller_id;
                //    item.status = questionDetail.status;
                //    item.text = questionDetail.text;
                //    item.ProductInfo = productDetail;
                //}

                //Questions = questions.questions
                //    .GroupBy(c => new
                //    {
                //        id = c.ProductInfo.id,
                //        title = c.ProductInfo.title,
                //        price = c.ProductInfo.price,
                //        thumbnail = c.ProductInfo.thumbnail,
                //        available_quantity = c.ProductInfo.available_quantity
                //    })

                //    .Select(x => new QuestionGroup()
                //    {
                //        Produto = new Item() { id = x.Key.id, title = x.Key.title, price = x.Key.price, thumbnail = x.Key.thumbnail, available_quantity = x.Key.available_quantity },
                //        Perguntas = x.Select(q => new ProductQuestionContent()
                //        {
                //            answer = q.answer,
                //            date_created = q.date_created,
                //            id = q.id,
                //            item_id = q.item_id,
                //            seller_id = q.seller_id,
                //            status = q.status,
                //            text = q.text
                //        }).ToList()
                //    })
                //    .ToList();

                //RaisePropertyChanged(() => Questions);

            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        private string _QuestionId;
        public string QuestionId
        {
            get { return _QuestionId; }
            set { Set(() => QuestionId, ref _QuestionId, value); }
        }

        private string _ProductId;
        public string ProductId
        {
            get { return _ProductId; }
            set { Set(() => ProductId, ref _ProductId, value); }
        }

        private ProductQuestionContent _QuestionInfo;
        public ProductQuestionContent QuestionInfo
        {
            get { return _QuestionInfo; }
            set { Set(() => QuestionInfo, ref _QuestionInfo, value); }
        }

        private string _AnswerText;
        public string AnswerText
        {
            get { return _AnswerText; }
            set { Set(() => AnswerText, ref _AnswerText, value); }
        }

        public RelayCommand SendAnswer { get; set; }
        public RelayCommand<object> SelectProduct { get; set; }
        

    }
}

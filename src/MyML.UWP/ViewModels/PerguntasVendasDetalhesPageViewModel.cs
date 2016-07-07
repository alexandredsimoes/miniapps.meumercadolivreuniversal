using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MyML.UWP.Aggregattor;
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
            OpenUserInfo = new RelayCommand<long?>((o) =>
            {
                NavigationService.Navigate(typeof(VendedorInfoPage), o);
            });

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
            if (string.IsNullOrWhiteSpace(AnswerText))
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
            else
            {
                await
                    new MessageDialog("Não foi possível enviar sua resposta. Verifique se o item está ativo.",
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
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
            var id = questionId.Split(new[] { ';' });
            QuestionId = id[0];
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

                if (question != null)
                {
                    ProductId = question.item_id;
                    question.Item = await _mercadoLivreService.GetItemDetails(question.item_id,
                        new KeyValuePair<string, object>[]
                        {
                            new KeyValuePair<string, object>("attributes","title,thumbnail,price,available_quantity")
                        });

                    var userInfo = await _mercadoLivreService.GetUserInfo(id[1],
                        new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "nickname,id") });
                    question.nickname = userInfo?.nickname;
                    question.buyer_id = userInfo?.id;

                    QuestionInfo = question;
                }
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
        public RelayCommand<long?> OpenUserInfo { get; set; }


    }
}

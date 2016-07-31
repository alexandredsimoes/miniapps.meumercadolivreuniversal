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

                NavigationService.Navigate(typeof(PerguntasVendasDetalhesPage), $"{question.id};{question.buyer_id}");
            });

            Refresh = new RelayCommand(async () => await LoadQuestions());
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
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
            var id = long.Parse(obj.Id.ToString());
            var q = Questions.FirstOrDefault();

            var q2 = q.FirstOrDefault(c => c.id == id);
            if (q2 != null)
            {
                await Task.Delay(1000);
                q.Remove(q2);
            }
        }

        private async Task LoadQuestions()
        {
            if (!_dataService.IsAuthenticated())
            {
                //await new MessageDialog(_resourceLoader.GetString("MsgNotAuthenticated"),
                //    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                Questions?.Clear();
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
                if (questions?.questions != null)
                {
                    foreach (var item in questions.questions)
                    {
                        if (item.From != null)
                        {
                            //Tenta obter os detalhes da questao
                            item.From.UserInfo = await _mercadoLivreService.GetUserInfo(item.From.id.ToString(), new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "nickname,status,registration_date,seller_experience,id") });
                        }
                        
                        var productDetail = await _mercadoLivreService.GetItemDetails(item.item_id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "id,title,price,thumbnail") });

                        if (productDetail == null)
                        {
                            await new MessageDialog(_resourceLoader.GetString("PerguntasComprasPageMsgErrorLoadQuestionDetail"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
                            break;
                        }

                        item.date_created = item.date_created;
                        item.id = item.id;
                        item.seller_id = item.seller_id;
                        item.status = item.status;
                        item.text = item.text;
                        item.ProductInfo = productDetail;
                    }

                    var query = from item in questions.questions
                                group item by new
                                {
                                    title = item.ProductInfo.title,
                                    price = item.ProductInfo.price,
                                    thumbnail = item.ProductInfo.thumbnail
                                } into g
                                select new { GroupName = new Item() { title = g.Key.title, price = g.Key.price, thumbnail = g.Key.thumbnail }, Items = g.ToList() };


                    foreach (var g in query)
                    {
                        QuestionGroup2 info = new QuestionGroup2();
                        info.Key = g.GroupName;
                        foreach (var item in g.Items)
                        {
                            info.Add(new ProductQuestionContent()
                            {
                                text = item.text,
                                item_id = item.item_id,
                                status = item.status,
                                answer = item.answer,
                                id = item.id,
                                seller_id = item.seller_id,
                                date_created = item.date_created,
                                nickname = item.From.UserInfo.nickname,
                                buyer_experience = item.From.UserInfo.seller_experience,
                                registration_date = item.From.UserInfo.registration_date,
                                buyer_id = item.From.UserInfo.id
                            });
                        }
                        Questions.Add(info);
                    }
                    RaisePropertyChanged(() => Questions);
                }
                
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


        private ObservableCollection<QuestionGroup2> _Questions = new ObservableCollection<QuestionGroup2>();

        public ObservableCollection<QuestionGroup2> Questions
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

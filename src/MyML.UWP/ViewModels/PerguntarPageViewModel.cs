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
    public class PerguntarPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public PerguntarPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;
            EnviarPergunta = new RelayCommand(EnviarPerguntaExecute);
        }

        private async void EnviarPerguntaExecute()
        {
            if (string.IsNullOrWhiteSpace(_QuestionText) || _QuestionText.Length < 5)
            {
                await new MessageDialog(_resourceLoader.GetString("PerguntaPageEmptyQuestion"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
                return;
            }

            if (!_dataService.IsAuthenticated())
            {
                await new MessageDialog(_resourceLoader.GetString("ProductSearchResultPageViewModelQuestionMustLogged"),
                    _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                NavigationService.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.ContinuumNavigationTransitionInfo());
                return;
            }

            try
            {
                Shell.SetBusy(true, "Enviando pergunta");
                var questionResult = await _mercadoLivreService.AskQuestion(QuestionText, ProductId);

                if (questionResult != null)
                {
                    await new MessageDialog(_resourceLoader.GetString("ProductSearchResultPageViewModelQuestionSend"),
                            _resourceLoader.GetString("ApplicationTitle")).ShowAsync();

                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();                    
                }
                else
                    await new MessageDialog(_resourceLoader.GetString("ProductSearchResultPageViewModelQuestionNotSend"),
                        _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
            }
            finally
            {
                Shell.SetBusy(false);
            }
           
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (parameter != null)
            {
                //Lista as questões relacionadas a esse produto
                //QuestionsList = await _mercadoLivreService.ListQuestionsByProduct(parameter.ToString());
                ProductId = parameter.ToString();
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }


        private ProductQuestion _QuestionsList;
        public ProductQuestion QuestionsList
        {
            get { return _QuestionsList; }
            set { Set(() => QuestionsList, ref _QuestionsList, value); }
        }

        private string _QuestionText;
        public string QuestionText
        {
            get { return _QuestionText; }
            set { Set(() => QuestionText, ref _QuestionText, value); }
        }

        private string _ProductId;
        public string ProductId
        {
            get { return _ProductId; }
            set { Set(() => ProductId, ref _ProductId, value); }
        }

        public RelayCommand EnviarPergunta { get; private set; }
    }
}

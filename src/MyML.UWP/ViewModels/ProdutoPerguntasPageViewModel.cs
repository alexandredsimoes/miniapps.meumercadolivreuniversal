using MyML.UWP.Adapters;
using MyML.UWP.Adapters.Search;
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
    public class ProdutoPerguntasPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        public ProdutoPerguntasPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (parameter != null)
            {
                var id = parameter.ToString();
                await LoadQuestions(id);
            }
        }

        private  Task LoadQuestions(string id)
        {
            Items = new IncrementalSearchSource<QuestionDataSource, ProductQuestionContent>(0, 15, id);

            
            Items.LoadMoreItemsStarted += () =>
            {
                Views.Shell.SetBusy(true, "Carregando informações");              
            };

            Items.LoadMoreItemsCompleted += () =>
            {
                Views.Shell.SetBusy(false);              
            };

            return Task.CompletedTask;          
        }

        private IncrementalSearchSource<QuestionDataSource, ProductQuestionContent> _Items;

        public IncrementalSearchSource<QuestionDataSource, ProductQuestionContent> Items
        {
            get { return _Items; }
            set { Set(() => Items, ref _Items, value); }
        }

    }
}

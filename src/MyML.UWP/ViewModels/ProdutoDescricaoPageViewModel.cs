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
    public class ProdutoDescricaoPageViewModel : ViewModelBase
    {        
        private readonly IMercadoLivreService _mercadoLivreService;        
        private readonly ResourceLoader _resourceLoader;
        public ProdutoDescricaoPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader)
        {
            _mercadoLivreService = mercadoLivreService;        
            _resourceLoader = resourceLoader;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(parameter != null)
            {
                var id = parameter.ToString();
                await LoadDescriptions(id);
            }            
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task LoadDescriptions(string id)
        {
            try
            {
                Views.Shell.SetBusy(true);
                //Pega a descrição do produto
                var description = await _mercadoLivreService.GetProductDescrition(id);
                if (description != null)
                {
                    IsHtml = !string.IsNullOrWhiteSpace(description.text) && string.IsNullOrWhiteSpace(description.plain_text);
                    Description = IsHtml ? description.text : description.plain_text;
                }
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
           
        }

        private bool _IsHtml;
        public bool IsHtml
        {
            get
            {
                return _IsHtml;
            }
            set
            {
                Set(() => IsHtml, ref _IsHtml, value);
            }
        }

        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                Set(() => Description, ref _Description, value);
            }
        }
    }
}

using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using AppStudio.Uwp.Commands;
using MyML.UWP.Views;

namespace MyML.UWP.ViewModels
{
    public class CategoriaPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreServices;
        public CategoriaPageViewModel(IMercadoLivreService mercadoLivreServices)
        {
            _mercadoLivreServices = mercadoLivreServices;

            SelecionarCategoria = new RelayCommand<object>(async o =>
            {
                var obj = o as ChildrenCategory;

                if (obj != null)
                {
                    CategoryDetail = await _mercadoLivreServices.GetCategoryDetail(obj.id);
                    RaisePropertyChanged("CategoryDetail");
                    if (CategoryDetail.children_categories.Count == 0)
                    {
                        await NavigationService.NavigateAsync(typeof(BuscaPage), $"{obj.id}|category");
                    }
                    
                }
            });
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            if (parameter != null)
            {
                CategoryDetail = await _mercadoLivreServices.GetCategoryDetail(parameter.ToString());
                RaisePropertyChanged("CategoryDetail");
            }
        }

        public MLCategorySearchResult CategoryDetail { get; set; }
        public RelayCommand<object> SelecionarCategoria { get; set; }
    }
}

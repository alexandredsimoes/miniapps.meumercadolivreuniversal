using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.ViewModels;
using MyML.UWP.Models.Mercadolivre;
using Windows.UI.Xaml.Hosting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyML.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuscaPage : Page
    {
        private readonly BuscaPageViewModel _viewModel;
         
        public BuscaPage()
        {
            this.InitializeComponent();
            //NavigationCacheMode = NavigationCacheMode.Enabled;

            _viewModel = DataContext as BuscaPageViewModel;
            ProdutosListView.SelectionChanged += ProdutosListView_SelectionChanged;
           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back && _viewModel?.SelectedItem != null)
            {
                this.UpdateLayout();
                ProdutosListView.ScrollIntoView(_viewModel.SelectedItem, ScrollIntoViewAlignment.Leading);
            }
        }


        private void ProdutosListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            if (selector is ListView)
            {
                _viewModel.SelectedItem = selector.SelectedItem as Item;
            }
        }
    }
}

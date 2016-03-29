﻿using MyML.UWP.ViewModels;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyML.UWP.Views.Secure
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VenderPage : Page
    {
        private VenderPageViewModel _viewModel;
        public VenderPage()
        {
            this.InitializeComponent();
            _viewModel = DataContext as VenderPageViewModel;


        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.PartChanged += _viewModel_PartChanged;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.PartChanged -= _viewModel_PartChanged;
        }

        private void _viewModel_PartChanged(int actualPart)
        {
            var resourceName = $"Storyboard{actualPart}";
            if(Resources.Any(c=>c.Key.ToString() == resourceName))
            {
                var storyBoard = Resources[resourceName] as Storyboard;
                if (storyBoard != null)
                    storyBoard.Begin();

            }
        }
    }
}

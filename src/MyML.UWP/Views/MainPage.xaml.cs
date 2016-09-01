using System;
using MyML.UWP.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Services.Store.Engagement;

namespace MyML.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        double InitialManipulationPointX = 0;
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            //ManipulationMode = ManipulationModes.TranslateX;
            //ManipulationStarted += Shell_ManipulationStarted;
            //ManipulationDelta += Shell_ManipulationDelta;
            //ManipulationCompleted += Shell_ManipulationCompleted;

            //AdMediator_DESKTOP.Visibility = App.ExibirAds ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            //AdMediator_MOBILE.Visibility = App.ExibirAds ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            
        }

        private void Shell_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            InitialManipulationPointX = e.Position.X;
            Debug.WriteLine("X -> " + InitialManipulationPointX);
        }

        private void Shell_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
        }

        private void Shell_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            InitialManipulationPointX = e.Position.X;
            Debug.WriteLine("X -> " + InitialManipulationPointX);
        }

        public MainPageViewModel ViewModel { get { return DataContext as MainPageViewModel; } }       
    }
}

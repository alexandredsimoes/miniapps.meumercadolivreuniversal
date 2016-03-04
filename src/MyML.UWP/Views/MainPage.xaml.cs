using MyML.UWP.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyML.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;


            //AdMediator_DESKTOP.Visibility = App.ExibirAds ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            //AdMediator_MOBILE.Visibility = App.ExibirAds ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        public MainPageViewModel ViewModel { get { return DataContext as MainPageViewModel; } }       
    }
}

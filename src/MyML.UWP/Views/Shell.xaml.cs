using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyML.UWP.Views
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplitView
    public sealed partial class Shell : Page
    {
        double InitialManipulationPointX = 0;
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu { get { return Instance.MyHamburgerMenu; } }

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            Loaded += Shell_Loaded;
            ManipulationMode = ManipulationModes.TranslateX;
            ManipulationStarted += Shell_ManipulationStarted;
            ManipulationDelta += Shell_ManipulationDelta;
            ManipulationCompleted += Shell_ManipulationCompleted;

            //MyHamburgerMenu.PaneOpened += (sender, e) =>
            //{
            //    SeparadorVendas.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //    SeparadorCompras.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //};
            //MyHamburgerMenu.PaneClosed += (sender, e) =>
            //{
            //    SeparadorVendas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //    SeparadorCompras.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //};
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

        private void Shell_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SettingsButton.IsEnabled = false;
        }

        public Shell(INavigationService navigationService)
        {
            Instance = this;
            InitializeComponent();
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }

        public static void SetBusy(bool busy, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                Instance.BusyView.BusyText = "aguarde...";
                Instance.ModalContainer.IsModal = Instance.BusyView.IsBusy = busy;
            });
        }

        private void MyHamburgerMenu_PaneClosed(object sender, System.EventArgs e)
        {
            SeparadorVendas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            SeparadorCompras.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void MyHamburgerMenu_PaneOpened(object sender, System.EventArgs e)
        {
            SeparadorVendas.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SeparadorCompras.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}


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
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            MyHamburgerMenu.PaneOpened += (sender, e) =>
            {
                SeparadorVendas.Visibility = Windows.UI.Xaml.Visibility.Visible;
                SeparadorCompras.Visibility = Windows.UI.Xaml.Visibility.Visible;
            };
            MyHamburgerMenu.PaneClosed += (sender, e) =>
            {
                SeparadorVendas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SeparadorCompras.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            };
        }


        public Shell(INavigationService navigationService) : this()
        {
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
                Instance.BusyView.BusyText = text;
                Instance.ModalContainer.IsModal = Instance.BusyView.IsBusy = busy;
            });
        }
    }
}


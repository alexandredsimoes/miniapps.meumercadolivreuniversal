using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyML.UWP.Views
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplitView
    public sealed partial class Shell : Page
    {
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
                SeparadorConfiguracao.Visibility = Visibility.Visible;
            };
            MyHamburgerMenu.PaneClosed += (sender, e) =>
            {
                SeparadorVendas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SeparadorCompras.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SeparadorConfiguracao.Visibility = Visibility.Collapsed;
            };


            if (App.ExibirAds == false)
            {
                AdMediatorMobile.Visibility = Visibility.Collapsed;
                AdMediatorMobile.Opacity = 0;
            }
            else
            {
                var family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                if (family.Equals("Windows.Desktop") || family.Equals("Windows.Xbox") || family.Equals("Windows.IoT"))
                {
                    AdMediatorMobile.Height = 90;
                    AdMediatorMobile.Width = 728;
                }
                else if (family.Equals("Windows.Mobile"))
                {
                    AdMediatorMobile.Height = 50;
                    AdMediatorMobile.Width = 300;
                }
                else
                {
                    AdMediatorMobile.Height = 50;
                    AdMediatorMobile.Width = 300;
                }

                AdMediatorMobile.Visibility = Visibility.Visible;
                AdMediatorMobile.Opacity = 1;
            }
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


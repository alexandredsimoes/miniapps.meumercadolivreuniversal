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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyML.UWP.Usercontrols
{
    public sealed partial class AdsControl : UserControl
    {
        public AdsControl()
        {
            this.InitializeComponent();
            
            this.Loaded += AdsControl_Loaded;
            if (!App.ExibirAds)
            {
                AdMediator_MOBILE.Visibility = Visibility.Collapsed;
                AdMediator_DESKTOP.Visibility = Visibility.Collapsed;
            }            
        }

        private void AdsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ExibirAds)
            {
                var family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                if(family.Equals("Windows.Desktop") || family.Equals("Windows.Xbox") || family.Equals("Windows.IoT"))
                {
                    AdMediator_DESKTOP.Visibility = Visibility.Visible;
                    AdMediator_MOBILE.Visibility = Visibility.Collapsed;
                }
                else if (family.Equals("Windows.Mobile"))
                {
                    AdMediator_DESKTOP.Visibility = Visibility.Collapsed;
                    AdMediator_MOBILE.Visibility = Visibility.Visible;
                }
                else
                {
                    AdMediator_DESKTOP.Visibility = Visibility.Collapsed;
                    AdMediator_MOBILE.Visibility = Visibility.Visible;
                }
            }           
        }
    }
}

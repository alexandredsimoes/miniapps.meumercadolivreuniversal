using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (!App.ExibirAds)
            {
                AdMediatorMobile.Visibility = Visibility.Collapsed;
                AdMediatorMobile.Opacity = 0;
            }
            else
            {
                AdMediatorMobile.Visibility = Visibility.Visible;
                AdMediatorMobile.Opacity = 1;

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
            }
        }        
    }
}

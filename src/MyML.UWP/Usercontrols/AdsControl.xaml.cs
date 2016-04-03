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


            AdMediator_MOBILE.AdUnitId = "186431";
            AdMediator_MOBILE.AppKey = "f5a5c89e-0fc3-492f-95c8-e11395eb3e6a";
            if (!App.ExibirAds)
            {
                AdMediator_MOBILE.Visibility = Visibility.Collapsed;
                AdMediator_MOBILE.Opacity = 0;
                //AdMediator_MOBILE.Pause();
                //AdMediator_DESKTOP.Visibility = Visibility.Collapsed;
            }
            else
            {
                //AdMediator_MOBILE.AdSdkTimeouts[AdSdkNames.MicrosoftAdvertising] = TimeSpan.FromSeconds(30);
                //AdMediator_MOBILE.AdSdkTimeouts[AdSdkNames.AdDuplex] = TimeSpan.FromSeconds(30);
                var family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                if (family.Equals("Windows.Desktop") || family.Equals("Windows.Xbox") || family.Equals("Windows.IoT"))
                {
                    AdMediator_MOBILE.Height = 90;
                    AdMediator_MOBILE.Width = 728;
                    //AdMediator_DESKTOP.Visibility = Visibility.Visible;
                    //AdMediator_MOBILE.Visibility = Visibility.Collapsed;
                    //AdMediator_MOBILE.Pause();
                }
                else if (family.Equals("Windows.Mobile"))
                {
                    AdMediator_MOBILE.Height = 50;
                    AdMediator_MOBILE.Width = 300;
                    //AdMediator_DESKTOP.Visibility = Visibility.Collapsed;
                    //AdMediator_MOBILE.Visibility = Visibility.Visible;
                    //AdMediator_DESKTOP.Pause();
                }
                else
                {
                    AdMediator_MOBILE.Height = 50;
                    AdMediator_MOBILE.Width = 300;
                }

                //AdMediator_DESKTOP.AdMediatorError += AdMediator_DESKTOP_AdMediatorError;
                //AdMediator_DESKTOP.AdSdkError += AdMediator_DESKTOP_AdSdkError;
                //AdMediator_MOBILE.AdMediatorError += AdMediator_MOBILE_AdMediatorError;
                //AdMediator_MOBILE.AdSdkError += AdMediator_MOBILE_AdSdkError;

            }
        }

        //private void AdMediator_MOBILE_AdSdkError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
        //{
        //    if(Debugger.IsAttached)
        //    {
        //        WriteErrorAdFailure("Mobile", e);
        //    }
        //}

        //private void WriteErrorAdFailure(string origin, AdFailedEventArgs e)
        //{
        //    Debug.WriteLine($"Mobile SDK Error - {origin} - {e.ErrorCode} - {e.Error?.Message} - {e.EventName} - {e.SdkEventArgs}");
        //}
        //private void WriteErrorMediatorFailure(string origin, AdMediatorFailedEventArgs e)
        //{
        //    Debug.WriteLine($"Mobile AdMediator Error - {origin} - {e.ErrorCode} - {e.Error?.Message}");
        //}

        //private void AdMediator_MOBILE_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        //{
        //    if (Debugger.IsAttached)
        //    {
        //        WriteErrorMediatorFailure("Mobile", e);
        //    }
        //}

        //private void AdMediator_DESKTOP_AdSdkError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
        //{
        //    if (Debugger.IsAttached)
        //    {
        //        WriteErrorAdFailure("Desktop", e);
        //    }
        //}

        //private void AdMediator_DESKTOP_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        //{
        //    if (Debugger.IsAttached)
        //    {
        //        WriteErrorMediatorFailure("Desktop", e);
        //    }
        //}

        //private void AdsControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (App.ExibirAds)
        //    {
        //        var family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
        //        if (family.Equals("Windows.Desktop") || family.Equals("Windows.Xbox") || family.Equals("Windows.IoT"))
        //        {
        //            AdMediator_MOBILE.Height = 90;
        //            AdMediator_MOBILE.Width = 728;
        //            //AdMediator_DESKTOP.Visibility = Visibility.Visible;
        //            //AdMediator_MOBILE.Visibility = Visibility.Collapsed;
        //            //AdMediator_MOBILE.Pause();
        //        }
        //        else if (family.Equals("Windows.Mobile"))
        //        {
        //            AdMediator_MOBILE.Height = 50;
        //            AdMediator_MOBILE.Width = 300;
        //            //AdMediator_DESKTOP.Visibility = Visibility.Collapsed;
        //            //AdMediator_MOBILE.Visibility = Visibility.Visible;
        //            //AdMediator_DESKTOP.Pause();
        //        }
        //        else
        //        {
        //            AdMediator_MOBILE.Height = 50;
        //            AdMediator_MOBILE.Width = 300;
        //        }
        //    }           
        //}
    }
}

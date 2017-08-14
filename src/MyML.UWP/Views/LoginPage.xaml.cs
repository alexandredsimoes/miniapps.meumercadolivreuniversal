using GalaSoft.MvvmLight.Threading;
using MyML.UWP.AppStorage;
using MyML.UWP.Models;
using MyML.UWP.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyML.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            LoginWebview.NavigationCompleted += LoginWebview_NavigationCompleted;
            LoginWebview.NavigationStarting += LoginWebview_NavigationStarting;
            //LoginWebview.Navigate(new Uri(Consts.GetUrl(Consts.ML_URL_AUTHENTICATION, Consts.ML_CLIENT_ID, "")));
        }

        private async void LoginWebview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            Views.Shell.SetBusy(true, "Aguarde enquanto o login é processado");
            if (args.Uri != null)
            {


#if DEBUG
                Debug.WriteLine("INICIANDO ->" + args.Uri.ToString());
                Debug.WriteLine("Navegando " + args.Uri.Query);
#endif
                //access_token=APP_USR-8765232316929095-081316-c8072b2ada1fef719b2af02be3f189f8__I_K__-41654723&expires_in=21600&user_id=41654723&domains=www.miniapps.com.br
                if (args.Uri.OriginalString.Contains("#access_token"))
                {
                    var query = args.Uri.AbsoluteUri.Substring(args.Uri.AbsoluteUri.IndexOf('#') + 1).Split( '&', '=');

                    var userInfo = new MLAutorizationInfo()
                    {
                        Access_Token = query[1],
                        Expires_In = Double.Parse(query[3]),
                        user_id  = query[5]
                    };
                    await ViewModel.SaveAuthenticationInfo(userInfo);
                }
            }
        }

        private async void LoginWebview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
           
        }

        public LoginPageViewModel ViewModel { get { return DataContext as LoginPageViewModel; } }
    }
}

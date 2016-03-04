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
                var code = String.Empty;
                var match = Regex.Match(args.Uri.Query, @"(code=)TG-(\W+|\w+)-\d+");

                if (match.Success)
                {
                    code = match.Value.Split('=')[1];
                }

                if (!String.IsNullOrWhiteSpace(code))
                {

                    try
                    {
#if DEBUG
                        Debug.WriteLine("Codigo validado " + code);
#endif
                        //LoginWebview.Opacity = 0;
                        //LoginWebview.Stop();
                        List<KeyValuePair<string, string>> parametros = new List<KeyValuePair<string, string>>();

                        parametros.Add(new KeyValuePair<string, string>("client_id", Consts.ML_CLIENT_ID));
                        parametros.Add(new KeyValuePair<string, string>("client_secret", Consts.ML_API_KEY));
                        parametros.Add(new KeyValuePair<string, string>("redirect_uri", Consts.ML_RETURN_URL));
                        parametros.Add(new KeyValuePair<string, string>("code", code));
                        parametros.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

#if DEBUG
                        Debug.WriteLine("Enviando codigo recebido " + code);
#endif
                        //var autorizationUrl = Consts.GetUrl(Consts.ML_AUTORIZATION_URL);
                        HttpClient httpClient = new HttpClient();
                        var result = await httpClient.PostAsync(Consts.ML_AUTORIZATION_URL, new FormUrlEncodedContent(parametros));
                        var content = await result.Content.ReadAsStringAsync();
                        var userInfo = JsonConvert.DeserializeObject<MLAutorizationInfo>(content);

#if DEBUG
                        Debug.WriteLine("Codigo validado " + content);
#endif
                        await ViewModel.SaveAuthenticationInfo(userInfo);

                    }
                    catch (Exception ex)
                    {
                        //Tenta converter para o objeto de erro
                        AppLogs.WriteError("LoginPage.NavigationStarting()", ex);

                        //Avisa o usuário sobre o erro
                        await new MessageDialog("Não foi possível conectar ao site do MercadoLivre. Verifique sua conexão com a internet.", "Meu MercadoLivre").ShowAsync();
                    }
                }
            }
        }

        private async void LoginWebview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            try
            {
                if (!args.IsSuccess)
                {
                    var message = String.Format("{0} - {1}", args.Uri.ToString(), args.WebErrorStatus.ToString());
                    AppLogs.WriteError("ERRO->LoginWebview_NavigationCompleted", message);

                    MessageDialog dialog = new MessageDialog("Não foi possível conectar ao site do MercadoLivre. Verifique sua conexão com a internet.", "Meu MercadoLivre");
                    await dialog.ShowAsync();
                }
                else
                {
                    Action clearRemoverControl = async () =>
                    {
                        try
                        {
                            var script = new List<string>();
                            script.Add("document.getElementsByName('remember_me')[0].removeAttribute('checked');");
                            await LoginWebview.InvokeScriptAsync("eval", script);

                            script.Clear();
                            script.Add("document.getElementsByName('remember_me')[0].parentNode.removeChild(document.getElementsByName('remember_me')[0]);");
                            await LoginWebview.InvokeScriptAsync("eval", script);
                        }
                        catch (Exception)
                        {
                        }

                    };
                    if (args.Uri.IsAbsoluteUri)
                    {
                        if (args.Uri.Query.Contains("errors.") || String.IsNullOrWhiteSpace(args.Uri.Query)) //Temos erro de autenticação
                        {
                            var message = String.Empty;

                            if (string.IsNullOrWhiteSpace(args.Uri.Query))
                            {
                                message = "O MercadoLivre está com dificuldades técnicas e não é possível efetuar o login nesse momento.";
                            }
                            else
                                message = "Não foi possível entrar. Verifique seu usuário ou senha.";

                            //_viewModel.IsBusy = false;

                            clearRemoverControl();


                            MessageDialog dialog = new MessageDialog(message, "Meu MercadoLivre");
                            await dialog.ShowAsync();

                            return;
                        }
                        else
                        {
                            if (args.Uri.ToString().Contains("/login"))
                            {
                                clearRemoverControl();
                            }
                        }
                    }
                    Views.Shell.SetBusy(false);
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("LoginWebView_NavigationCompleted", ex);
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        public LoginPageViewModel ViewModel { get { return DataContext as LoginPageViewModel; } }
    }
}

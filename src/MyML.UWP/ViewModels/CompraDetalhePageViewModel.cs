using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.AppStorage;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using MyML.UWP.Views.Secure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;

namespace MyML.UWP.ViewModels
{
    public class CompraDetalhePageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public CompraDetalhePageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;

            TrackOrder = new RelayCommand(async () =>
            {

                if (String.IsNullOrWhiteSpace(OrderInfo?.shipping?.tracking_number))
                {
                    await new MessageDialog("Nenhuma informação de rastreamento está disponível.").ShowAsync();
                    return;
                }
                var client = SimpleIoc.Default.GetInstance<HttpClient>();
                try
                {
                    string url = $"http://websro.correios.com.br/sro_bin/txect01$.QueryList?P_LINGUA=001&P_TIPO=001&P_COD_UNI={OrderInfo.shipping.tracking_number}";
                    var response = await client.GetAsync(url);
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var content = await response.Content.ReadAsStringAsync();                        
                        
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(content);
                        var nodes = doc.DocumentNode.ChildNodes.ToList();
                        var rows = doc.DocumentNode.ChildNodes["html"].Descendants("tr").ToList();

                        if(rows == null  || rows.Count == 0)
                        {
                            await new MessageDialog("Não há informações sobre rastreamento desse objeto. Tente novamente mais tarde",
                                _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                            return;
                        }


                        //TrackingText = $"<html><body>{htmlTable}</body></html>";
                        //return;
                        var linha = 1;
                        //StringBuilder saida = new StringBuilder();
                        TrackingList = new ObservableCollection<TrackingStatus>();
                        foreach (var row in rows)
                        {
                            if(linha > 1)
                            {
                                var textos = row.Descendants("#text").ToList();

                                var trackingList = new TrackingStatus();
                                for (int i = 0; i < textos.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        trackingList.Col1 = textos[i].InnerHtml;

                                        var td = textos[0].ParentNode;
                                        if (td?.OuterHtml.Contains("colspan=\"2\"") ?? false)
                                        {
                                            trackingList.ColSpan = 3;
                                            trackingList.Alignment = "Center";
                                        }
                                        else
                                        {
                                            trackingList.ColSpan = 1;
                                            trackingList.Alignment = "Left";
                                        }
                                    }
                                    if (i == 1) trackingList.Col2 = textos[i].InnerHtml;
                                    if (i == 2) trackingList.Col3 = textos[i].InnerHtml;
                                    
                                    //if (textos.Count == 1)
                                      //  TrackingList.Add(new TrackingStatus() { });
                                }
                                TrackingList.Add(trackingList);
                                //foreach (var item in textos)
                                //{
                                //    saida.Append($"{item.InnerHtml}\t");
                                //    if (textos.Count == 1)
                                //        saida.AppendLine();
                                //}
                                //saida.AppendLine();
                                // Debug.WriteLine(row.InnerHtml);
                                //var s = Windows.Data.Html.HtmlUtilities.ConvertToText(row.InnerHtml);
                                //Debug.WriteLine(s);
                            }
                            linha++;
                        }
                        //Debug.WriteLine(saida.ToString());
                        //TrackingText = saida.ToString();
                        RaisePropertyChanged(() => TrackingList);
                    }
                    
                    var uri = new Uri(url);
                    //await Launcher.LaunchUriAsync(uri);
                }
                finally
                {

                }               
            });
            BuyerAction = new RelayCommand<string>(BuyerActionExecute);
            QualifyProduct = new RelayCommand(() =>
            {
                NavigationService.Navigate(typeof(CompraQualificarPage), OrderInfo.id);
            });
            
            CopyTrackingCode = new RelayCommand<string>( async(o) =>
            {              
                var dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                if (o == "TrackingCode")
                {
                    if (String.IsNullOrWhiteSpace(OrderInfo?.shipping?.tracking_number))
                    {
                        await new MessageDialog("Nenhum código de rastreamento foi informado pelo vendedor", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                        return;
                    }
                    dataPackage.SetText(OrderInfo.shipping.tracking_number);
                }
                else if (o == "Email")
                {
                    if (String.IsNullOrWhiteSpace(OrderInfo?.seller?.email))
                    {
                        await new MessageDialog("Nenhum e-mail foi informado pelo vendedor", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                        return;
                    }
                    dataPackage.SetText(OrderInfo.seller.email);
                }
                Clipboard.SetContent(dataPackage);
                await new MessageDialog("Informação copiada para Área de transferência.", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();               
                
            });
        }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(mode != NavigationMode.Back)
            {
                if(parameter != null)
                {
                    TrackingList?.Clear();
                    OrderId = parameter.ToString();
                    await LoadOrderInfo(OrderId);
                }                
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private async void BuyerActionExecute(string arg)
        {
            if(OrderInfo == null || string.IsNullOrWhiteSpace(arg))
            {
                await new MessageDialog("Nenhuma informação de compra foi carregada", "Ação não disponível").ShowAsync();
                return;
            }
            var phoneNumber = string.Format("{0}{1}", OrderInfo.seller?.phone?.area_code, OrderInfo.seller?.phone?.number);
            var buyerName = OrderInfo.seller?.first_name;
            var buyerEmail = OrderInfo.seller?.email;


            if (arg == "call")
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.ApplicationModel.Calls.PhoneCallManager"))
                {
                    Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(phoneNumber, buyerName);
                }
                else
                    await new MessageDialog(_resourceLoader.GetString("MsgDeviceCallNotFound"), _resourceLoader.GetString("ApplicationName")).ShowAsync();
            }
            else if (arg == "email")
            {
                if (!string.IsNullOrWhiteSpace(buyerEmail) && !string.IsNullOrWhiteSpace(buyerName))
                {
                    var emailRecipient = new EmailRecipient
                    {
                        Address = buyerEmail,
                        Name = buyerName
                    };

                    var item = OrderInfo.order_items[0]?.item;
                    if (item != null)
                        await EmailManager.ShowComposeNewEmailAsync(new EmailMessage()
                        {
                            Subject = item?.title,
                            Sender = emailRecipient
                        });
                }
            }
            else
            {
                if (buyerName != null && buyerEmail != null)
                {
                    var contact = new Contact
                    {
                        FirstName = buyerName
                    };

                    if (!string.IsNullOrEmpty(buyerEmail))
                    {
                        var homeEmail = new ContactEmail
                        {
                            Address = buyerEmail,
                            Kind = ContactEmailKind.Work
                        };
                        contact.Emails.Add(homeEmail);
                    }
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        var workPhone = new ContactPhone
                        {
                            Number = phoneNumber,
                            Kind = ContactPhoneKind.Work
                        };
                        contact.Phones.Add(workPhone);
                    }

                    ContactManager.ShowFullContactCard(contact,
                        new FullContactCardOptions() {DesiredRemainingView = ViewSizePreference.Default});
                    //ContactManager.ShowContactCard(contact, new Rect(), Placement.Above);
                }                
            }
        }

        private async Task LoadOrderInfo(string orderId)
        {
            try
            {
                Shell.SetBusy(true, "Carregando informações");
                var orderInfo = await _mercadoLivreService.GetOrderDetail(orderId);

                if (orderInfo != null)
                {
                    
                    var feedbackInfo = await _mercadoLivreService.GetOrderFeedback(orderInfo.id.ToString());
                    //orderInfo.feedback = feedbackInfo;

                    //Pega os dados de entrega
                    if (orderInfo.shipping != null)
                    {
                        var shippingDetails = await _mercadoLivreService.GetShippingDetails(orderInfo.shipping.id.ToString());

                        DateTime data;
                        if (shippingDetails != null && DateTime.TryParse(shippingDetails.status_history.date_delivered, out data))
                            shippingDetails.status_history.date_delivered = data.ToString("dd/MMMM/yyyy");

                        orderInfo.shipping = shippingDetails;
                    }

                    if (orderInfo.feedback != null)
                    {
                        HasPurchaseFeedback = orderInfo.feedback.purchase != null;
                        HasSaleFeedback = orderInfo.feedback.sale != null;

                        if(feedbackInfo != null)
                        {
                            if (feedbackInfo.purchase != null)
                                orderInfo.feedback.purchase = feedbackInfo.purchase;

                            if (feedbackInfo.sale != null)
                                orderInfo.feedback.sale = feedbackInfo.sale;
                        }                       
                    }
                    OrderInfo = orderInfo;
                }               
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("CompraDetalhePageViewModel.LoadOrderInfo()", ex);
                await new MessageDialog("Não foi possível obter os dados da compra.", "Erro").ShowAsync();
            }
            finally
            {
                Shell.SetBusy(false);
            }
        }

        private string _orderId;
        public string OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        private MLOrderInfo _orderInfo;
        public MLOrderInfo OrderInfo
        {
            get { return _orderInfo; }
            set { Set(() => OrderInfo, ref _orderInfo, value); }
        }

        private bool _hasSaleFeedback = false;
        public bool HasSaleFeedback
        {
            get { return _hasSaleFeedback; }
            set { Set(() => HasSaleFeedback, ref _hasSaleFeedback, value); }
        }

        private bool _hasPurchaseFeedback = false;
        public bool HasPurchaseFeedback
        {
            get { return _hasPurchaseFeedback; }
            set { Set(() => HasPurchaseFeedback, ref _hasPurchaseFeedback, value); }
        }
        private string _trackingText = "";
        public string TrackingText
        {
            get { return _trackingText; }
            set { Set(() => TrackingText, ref _trackingText, value); }
        }

        private ObservableCollection<TrackingStatus>  _trackingList;
        public ObservableCollection<TrackingStatus> TrackingList
        {
            get { return _trackingList; }
            set { Set(() => TrackingList, ref _trackingList, value); }
        }



        public RelayCommand<string> BuyerAction { get; private set; }
        public RelayCommand QualifyProduct { get; private set; }
        public RelayCommand<string> CopyTrackingCode { get; private set; }
        public RelayCommand TrackOrder { get; set; }

    }

    public struct TrackingStatus
    {
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public int ColSpan { get; set; }
        public string Alignment { get; set; }
    }
}

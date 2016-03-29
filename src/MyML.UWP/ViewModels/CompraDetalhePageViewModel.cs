using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.AppStorage;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using MyML.UWP.Views;
using MyML.UWP.Views.Secure;
using System;
using System.Collections.Generic;
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
using Windows.UI.Popups;
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
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(mode != NavigationMode.Back)
            {
                if(parameter != null)
                {
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
            if(OrderInfo == null)
            {
                await new MessageDialog("Nenhuma informação de compra foi carregada", "Ação não disponível").ShowAsync();
                return;
            }
            var phoneNumber = String.Format("{0}{1}", OrderInfo.seller.phone.area_code, OrderInfo.seller.phone.number);
            var buyerName = OrderInfo.seller.first_name;
            var buyerEmail = OrderInfo.seller.email;


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
                var emailRecipient = new EmailRecipient();
                emailRecipient.Address = buyerEmail;
                emailRecipient.Name = buyerName;

                await EmailManager.ShowComposeNewEmailAsync(new EmailMessage()
                {
                    Subject = OrderInfo.order_items[0].item.title,
                    Sender = emailRecipient
                });
            }
            else
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

                ContactManager.ShowContactCard(contact, new Rect(), Placement.Above);

                //await _alertMessageService.ShowAsync(_resourceLoader.GetString("OrdersPageViewModelMsgAddContact"),
                    //_resourceLoader.GetString("ApplicationTitle"), commands);
            }
        }

        private async Task LoadOrderInfo(string orderId)
        {
            try
            {
                Shell.SetBusy(true, "Carregando informações");
                var data = DateTime.MinValue;
                var orderInfo = await _mercadoLivreService.GetOrderDetail(orderId);

                if (orderInfo != null)
                {
                    
                    var feedbackInfo = await _mercadoLivreService.GetOrderFeedback(orderInfo.id.ToString());
                    //orderInfo.feedback = feedbackInfo;

                    //Pega os dados de entrega
                    if (orderInfo.shipping != null)
                    {
                        var shippingDetails = await _mercadoLivreService.GetShippingDetails(orderInfo.shipping.id.ToString());

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
                AppLogs.WriteError("CompraDetalhePageViewModel.LoadOrderInfo()", ex);
                await new MessageDialog("Não foi possível obter os dados da compra.", "Erro").ShowAsync();
            }
            finally
            {
                Shell.SetBusy(false);
            }
        }

        private string _OrderId;
        public string OrderId
        {
            get { return _OrderId; }
            set { _OrderId = value; }
        }

        private MLOrderInfo _OrderInfo;
        public MLOrderInfo OrderInfo
        {
            get { return _OrderInfo; }
            set { Set(() => OrderInfo, ref _OrderInfo, value); }
        }

        private bool _HasSaleFeedback = false;
        public bool HasSaleFeedback
        {
            get { return _HasSaleFeedback; }
            set { Set(() => HasSaleFeedback, ref _HasSaleFeedback, value); }
        }

        private bool _HasPurchaseFeedback = false;
        public bool HasPurchaseFeedback
        {
            get { return _HasPurchaseFeedback; }
            set { Set(() => HasPurchaseFeedback, ref _HasPurchaseFeedback, value); }
        }


        public RelayCommand<string> BuyerAction { get; private set; }
        public RelayCommand QualifyProduct { get; private set; }
        public RelayCommand<string> CopyTrackingCode { get; private set; }

    }
}

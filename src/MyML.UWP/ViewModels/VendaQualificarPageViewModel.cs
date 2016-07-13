using GalaSoft.MvvmLight.Command;
using MyML.UWP.Services;
using MyML.UWP.Views;
using MyML.UWP.Views.Secure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.AppStorage;
using Template10.Services.NavigationService;

namespace MyML.UWP.ViewModels
{
    public class VendaQualificarPageViewModel : ViewModelBase
    {
        private readonly IMercadoLivreService _mercadoLivreService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IDataService _dataService;
        public VendaQualificarPageViewModel(IMercadoLivreService mercadoLivreService, ResourceLoader resourceLoader,
            IDataService dataService)
        {
            _mercadoLivreService = mercadoLivreService;
            _resourceLoader = resourceLoader;
            _dataService = dataService;

            NextPage = new RelayCommand(() =>
            {
                if (IsFulFilled ?? false)
                {
                    if (ActualPart == "QualifyPartOne")
                        ActualPart = "QualifyPartTwoYes";
                    else if (ActualPart == "QualifyPartTwoYes")
                        ActualPart = "QualifyPartThreeYes";
                }
                else
                {
                    if (ActualPart == "QualifyPartOne")
                        ActualPart = "QualifyPartTwoNo";
                    else if (ActualPart == "QualifyPartTwoNo")
                        ActualPart = "QualifyPartTwoYes";
                    else if (ActualPart == "QualifyPartTwoYes")
                        ActualPart = "QualifyPartThreeYes";
                }
            });
            ResetRating = new RelayCommand(() => { Rating = null; });

            Qualify = new RelayCommand(QualifyExecute);
        }

        private async void QualifyExecute()
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                await new MessageDialog("Informe sua mensagem antes de qualificar", _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                return;
            }

            if (IsFulFilled ?? false)
            {
                SendFulFilledFeedback();
            }
            else
            {
                SendFulNotFilledFeedback();
            }
        }

        private async void SendFulNotFilledFeedback()
        {
            try
            {
                Views.Shell.SetBusy(true, "Enviando feedback");
                MLRating rating = Rating == "positive" ? MLRating.positive : Rating == "negative" ? MLRating.negative : MLRating.neutral;
                MLSellerRatingReason reason = (MLSellerRatingReason)Enum.Parse(typeof(MLSellerRatingReason), _Reason, false);


                var result = await _mercadoLivreService.SendSellerOrderFeedback(OrderInfo.id.ToString(),
                    false, rating, Message, reason, RestockItem ?? false, HasSellerRefundMoney ?? false);
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("VendaQualificarPageViewMode.OnNavigatedToAsync()", ex);
                await new MessageDialog("Erro ao enviar qualificação. Envie o log de erros para o desenvolvedor.").ShowAsync();
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        private async void SendFulFilledFeedback()
        {
            try
            {
                MLRating rating = Rating == "positive" ? MLRating.positive : Rating == "negative" ? MLRating.negative : MLRating.neutral;
                
                Views.Shell.SetBusy(true, "Enviando feedback");
                var result = await _mercadoLivreService.SendSellerOrderFeedback(OrderInfo.id.ToString(),
                    true, rating, Message, MLSellerRatingReason.BUYER_NOT_ENOUGH_MONEY /*Para vendedores não tem efeito*/);                
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("VendaQualificarPageViewMode.OnNavigatedToAsync()", ex);
                await new MessageDialog("Erro ao enviar qualificação. Envie o log de erros para o desenvolvedor.").ShowAsync();
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(parameter == null)
            {
                await new MessageDialog("Nenhuma informação de venda foi informada", "Erro").ShowAsync();
                NavigationService.GoBack();
                return;
            }
            try
            {
                Views.Shell.SetBusy(true);
                ActualPart = "QualifyPartOne";
                //Carrega os dados da order
                OrderInfo = await _mercadoLivreService.GetOrderDetail(parameter.ToString());
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("VendaQualificarPageViewMode.OnNavigatedToAsync()", ex);
            }
            finally
            {
                Views.Shell.SetBusy(false);
            }

        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if(args.NavigationMode == NavigationMode.Back && !args.Suspending)
            {
                ActualPart = "QualifyPartOne";
                IsFulFilled = null;
                Rating = null;
                Reason = null;
                OrderInfo = null;
                Message = null;
                HasSellerRefundMoney = null;
                RestockItem = null;
            }

            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private bool? _IsFulfilled = null;
        public bool? IsFulFilled
        {
            get { return _IsFulfilled; }
            set { Set(() => IsFulFilled, ref _IsFulfilled, value); }
        }

        private string _Rating = null; 
        public string Rating
        {
            get { return _Rating; }
            set { Set(() => Rating, ref _Rating, value); }
        }

        private string _Reason = null;
        public string Reason
        {
            get { return _Reason; }
            set { Set(() => Reason, ref _Reason, value); }
        }

        private string _ActualPart = "QualifyPartOne";
        public string ActualPart
        {
            get { return _ActualPart; }
            set { Set(() => ActualPart, ref _ActualPart, value); }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { Set(() => Message, ref _Message, value); }
        }

        private MLOrderInfo _OrderInfo;
        public MLOrderInfo OrderInfo
        {
            get { return _OrderInfo; }
            set { Set(() => OrderInfo, ref _OrderInfo, value); }
        }

        private bool? _HasSellerRefundMoney;
        public bool? HasSellerRefundMoney
        {
            get { return _HasSellerRefundMoney; }
            set { Set(() => HasSellerRefundMoney, ref _HasSellerRefundMoney, value); }
        }

        private bool? _RestockItem;
        public bool? RestockItem
        {
            get { return _RestockItem; }
            set { Set(() => RestockItem, ref _RestockItem, value); }
        }






        public RelayCommand NextPage { get; private set; }
        public RelayCommand Qualify { get; private set; }
        public RelayCommand ResetRating { get; private set; }

    }
}

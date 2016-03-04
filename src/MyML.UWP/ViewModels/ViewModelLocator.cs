using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MyML.UWP.Models;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace MyML.UWP.ViewModels
{
    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<IMercadoLivreService, MercadoLivreServices>();
            SimpleIoc.Default.Register(() => new ResourceLoader());
            SimpleIoc.Default.Register(() => new HttpClient(), false);
            

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<DetailPageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<BuscaPageViewModel>();
            SimpleIoc.Default.Register<ProdutoDetalheViewModel>();
            SimpleIoc.Default.Register<PerguntarPageViewModel>();
            SimpleIoc.Default.Register<PerguntasComprasPageViewModel>();
            SimpleIoc.Default.Register<PerguntasVendasPageViewModel>();
            SimpleIoc.Default.Register<ComprasPageViewModel>();
            SimpleIoc.Default.Register<CompraDetalhePageViewModel>();
            SimpleIoc.Default.Register<AnunciosPageViewModel>();
            SimpleIoc.Default.Register<AnunciosDetalhePageViewModel>();
            SimpleIoc.Default.Register<AnunciosAlterarExposicaoPageViewModel>();
            SimpleIoc.Default.Register<AlterarAtributoPageViewModel>();
            SimpleIoc.Default.Register<FavoritesPageViewModel>();
            SimpleIoc.Default.Register<ComprarItemPageViewModel>();
            SimpleIoc.Default.Register<VendasPageViewModel>();
            SimpleIoc.Default.Register<VendaDetalhePageViewModel>();
            SimpleIoc.Default.Register<PerguntasVendasDetalhesPageViewModel>();
            SimpleIoc.Default.Register<BuscaFilterPageViewModel>();
            SimpleIoc.Default.Register<ProdutoDescricaoPageViewModel>();
            SimpleIoc.Default.Register<ProdutoPerguntasPageViewModel>();
            SimpleIoc.Default.Register<VendedorInfoPageViewModel>();
            SimpleIoc.Default.Register<VendaQualificarPageViewModel>();
            SimpleIoc.Default.Register<CompraQualificarPageViewModel>();            
        }

        public MainPageViewModel MainPage => ServiceLocator.Current.GetInstance<MainPageViewModel>();
        public LoginPageViewModel LoginPage => ServiceLocator.Current.GetInstance<LoginPageViewModel>();
        public BuscaPageViewModel BuscaPage => ServiceLocator.Current.GetInstance<BuscaPageViewModel>();
        public ProdutoDetalheViewModel ProdutoDetalhePage => ServiceLocator.Current.GetInstance<ProdutoDetalheViewModel>();
        public PerguntarPageViewModel PerguntarPage => ServiceLocator.Current.GetInstance<PerguntarPageViewModel>();
        public PerguntasComprasPageViewModel PerguntasComprasPage => ServiceLocator.Current.GetInstance<PerguntasComprasPageViewModel>();
        public ComprasPageViewModel ComprasPage => ServiceLocator.Current.GetInstance<ComprasPageViewModel>();
        public CompraDetalhePageViewModel CompraDetalhePage => ServiceLocator.Current.GetInstance<CompraDetalhePageViewModel>();
        public AnunciosPageViewModel AnunciosPage => ServiceLocator.Current.GetInstance<AnunciosPageViewModel>();
        public AnunciosDetalhePageViewModel AnunciosDetalhePage => ServiceLocator.Current.GetInstance<AnunciosDetalhePageViewModel>();
        public AnunciosAlterarExposicaoPageViewModel AnunciosAlterarExposicaoPage => ServiceLocator.Current.GetInstance<AnunciosAlterarExposicaoPageViewModel>();
        public AlterarAtributoPageViewModel AlterarAtributoPage => ServiceLocator.Current.GetInstance<AlterarAtributoPageViewModel>();
        public FavoritesPageViewModel FavoritesPage => ServiceLocator.Current.GetInstance<FavoritesPageViewModel>();
        public ComprarItemPageViewModel ComprarItemPage => ServiceLocator.Current.GetInstance<ComprarItemPageViewModel>();
        public VendasPageViewModel VendasPage => ServiceLocator.Current.GetInstance<VendasPageViewModel>();
        public VendaDetalhePageViewModel VendaDetalhePage => ServiceLocator.Current.GetInstance<VendaDetalhePageViewModel>();
        public PerguntasVendasPageViewModel PerguntasVendasPage => ServiceLocator.Current.GetInstance<PerguntasVendasPageViewModel>();
        public PerguntasVendasDetalhesPageViewModel PerguntasVendasDetalhesPage => ServiceLocator.Current.GetInstance<PerguntasVendasDetalhesPageViewModel>();
        public BuscaFilterPageViewModel BuscaFilterPage => ServiceLocator.Current.GetInstance<BuscaFilterPageViewModel>();
        public ProdutoDescricaoPageViewModel ProdutoDescricaoPage => ServiceLocator.Current.GetInstance<ProdutoDescricaoPageViewModel>();
        public ProdutoPerguntasPageViewModel ProdutoPerguntasPage => ServiceLocator.Current.GetInstance<ProdutoPerguntasPageViewModel>();
        public VendedorInfoPageViewModel VendedorInfoPage => ServiceLocator.Current.GetInstance<VendedorInfoPageViewModel>();
        public VendaQualificarPageViewModel VendaQualificarPage => ServiceLocator.Current.GetInstance<VendaQualificarPageViewModel>();
        public CompraQualificarPageViewModel CompraQualificarPage => ServiceLocator.Current.GetInstance<CompraQualificarPageViewModel>();        
    }
}

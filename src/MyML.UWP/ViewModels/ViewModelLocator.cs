﻿using GalaSoft.MvvmLight.Ioc;
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

            //if (!GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            //{                
                SimpleIoc.Default.Register<IDataService, DataService>();
                SimpleIoc.Default.Register<IMercadoLivreService, MercadoLivreServices>();
                SimpleIoc.Default.Register(() => new ResourceLoader());
                SimpleIoc.Default.Register(() => new HttpClient(), false);
            //}
            //else
            //{
            //    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            //    SimpleIoc.Default.Register<IMercadoLivreService, DesignMercadoLivreServices>();                
            //}

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
            SimpleIoc.Default.Register<VenderPageViewModel>();
            SimpleIoc.Default.Register<ProdutoDetalheEnvioPageViewModel>();
            SimpleIoc.Default.Register<OrdernarBuscaPageViewModel>();
            SimpleIoc.Default.Register<VendasArquivadasPageViewModel>();
            SimpleIoc.Default.Register<SaldoPageViewModel>();
            SimpleIoc.Default.Register<ProdutoUsuarioPageViewModel>();
            SimpleIoc.Default.Register<CategoriaPageViewModel>();
            SimpleIoc.Default.Register<DadosPessoaisPageViewModel>();
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
        public VenderPageViewModel VenderPage => ServiceLocator.Current.GetInstance<VenderPageViewModel>();
        public ProdutoDetalheEnvioPageViewModel ProdutoDetalheEnvioPage => ServiceLocator.Current.GetInstance<ProdutoDetalheEnvioPageViewModel>();
        public SettingsPageViewModel SettingsPage => ServiceLocator.Current.GetInstance<SettingsPageViewModel>();
        public OrdernarBuscaPageViewModel OrdernarBuscaPage => ServiceLocator.Current.GetInstance<OrdernarBuscaPageViewModel>();
        public VendasArquivadasPageViewModel VendasArquivadasPage => ServiceLocator.Current.GetInstance<VendasArquivadasPageViewModel>();
        public SaldoPageViewModel SaldoPage => ServiceLocator.Current.GetInstance<SaldoPageViewModel>();
        public ProdutoUsuarioPageViewModel ProdutoUsuarioPage => ServiceLocator.Current.GetInstance<ProdutoUsuarioPageViewModel>();
        public CategoriaPageViewModel CategoriaPage => ServiceLocator.Current.GetInstance<CategoriaPageViewModel>();
        public DadosPessoaisPageViewModel DadosPessoaisPage => ServiceLocator.Current.GetInstance<DadosPessoaisPageViewModel>();        

    }
}

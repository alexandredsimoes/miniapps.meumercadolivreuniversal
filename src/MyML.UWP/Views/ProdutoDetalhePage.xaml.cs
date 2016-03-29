﻿using MyML.UWP.ViewModels;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyML.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProdutoDetalhePage : Page
    {
        private ProdutoDetalheViewModel _viewModel;

        public ProdutoDetalhePage()
        {
            this.InitializeComponent();

            _viewModel = DataContext as ProdutoDetalheViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.ZoomModeChanged += _viewModel_ZoomModeChanged;    
        }

        private void _viewModel_ZoomModeChanged(bool isZoomEnabled)
        {
            if (isZoomEnabled)
            {
                Storyboard2.Begin();
            }
            else
                Storyboard3.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _viewModel.ZoomModeChanged -= _viewModel_ZoomModeChanged;   
        }
    }
}

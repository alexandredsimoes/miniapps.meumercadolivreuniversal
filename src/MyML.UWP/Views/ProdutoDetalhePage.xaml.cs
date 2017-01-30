using MyML.UWP.ViewModels;
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
using Windows.UI.Xaml.Media.Media3D;
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
            ////////////////if (isZoomEnabled)
            ////////////////{
            ////////////////    Storyboard2.Begin();
            ////////////////}
            ////////////////else
            ////////////////    Storyboard3.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _viewModel.ZoomModeChanged -= _viewModel_ZoomModeChanged;   
        }

        private void NormalizeParallax(UIElement target)
        {
            var transform = target.Transform3D as CompositeTransform3D;

            if (transform != null)
            {
                var transformToVisual = ParallaxRoot.TransformToVisual(target);
                var center = new Point(ParallaxRoot.ActualWidth / 2.0, RootGrid.ActualHeight / 2.0);

                // Center of ParallaxRoot in the coordinates of target.
                var transformedCenter = transformToVisual.TransformPoint(center);

                transform.CenterX = transformedCenter.X;

                // TransformToVisual doesn't account for ScrollViewer offset
                transform.CenterY = transformedCenter.Y - ParallaxRoot. VerticalOffset;

                // This could be done statically in markup but it's easier to show here.
                transform.ScaleX = transform.ScaleY =
                    -transform.TranslateZ / PerspectiveTransform.Depth + 0.8;
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
             //NormalizeParallax(Carousel);
        }
    }
}

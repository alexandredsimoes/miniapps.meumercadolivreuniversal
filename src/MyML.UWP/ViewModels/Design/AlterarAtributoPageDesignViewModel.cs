using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MyML.UWP.Models;

namespace MyML.UWP.ViewModels.Design
{
    public class AlterarAtributoPageDesignViewModel
    {
        public AlterarAtributoPageDesignViewModel()
        {
            Pictures = new List<ProductImage>
            {
                new ProductImage()
                {
                    Source =
                        new BitmapImage(
                            new Uri(
                                "ms-appx:///assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg"))
                },
                new ProductImage()
                {
                    Source =
                        new BitmapImage(
                            new Uri(
                                "ms-appx:///assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg"))
                }
            };
        }

        public IList<ProductImage> Pictures { get; set; }
        public int AvailablePhotos => 3;
    }
}

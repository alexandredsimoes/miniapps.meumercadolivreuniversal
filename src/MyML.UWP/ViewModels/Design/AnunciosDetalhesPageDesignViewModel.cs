using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;

namespace MyML.UWP.ViewModels.Design
{
    public class AnunciosDetalhesPageDesignViewModel
    {
        public AnunciosDetalhesPageDesignViewModel()
        {


            Item = new UWP.Models.Mercadolivre.Item()
            {
                title = "Produto via designViewModel",
                price = 1789999D,
                sold_quantity = 2500,
                original_price = 200D,
                accepts_mercadopago = true,
                thumbnail = "ms-appx:///assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg",
                pictures = new List<Picture>() { new Picture() { url = "ms-appx:///assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg" },
                                                    new Picture() { url = "ms-appx:///assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg" }
                },
                seller_address = new SellerAddress() { city = new City() { name = "São Paulo" }, state = new State() { name = "SP" } },
                installments = new Installments() { amount = 52, quantity = 24 },
                available_quantity = 222
            };
        }

        public Item Item { get; set; }


    }
}

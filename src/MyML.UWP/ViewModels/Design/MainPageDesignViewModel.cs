using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.ViewModels.Design
{
    public class MainPageDesignViewModel
    {
        public MainPageDesignViewModel()
        {
            Items = new List<MLItemHomeFeature>()
            {
                new MLItemHomeFeature()
                {
                    title = "Produto XYZ - Teste",
                    price = 59.99,
                    picture = new Picture()
                    {
                        url = "ms-appx:///assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg"
                    }
                },
                new MLItemHomeFeature()
                {
                    title = "Produto XYZ - Teste",
                    price = 59.99,
                    picture = new Picture()
                    {
                        url = "ms-appx:///assets/chat-in.png"
                    }
                },
                new MLItemHomeFeature()
                {
                    title = "Produto XYZ - Teste",
                    price = 59.99,
                    picture = new Picture()
                    {
                        url = "ms-appx:///assets/me.png"
                    }
                },
                new MLItemHomeFeature()
                {
                    title = "Produto XYZ - Teste",
                    price = 59.99,
                    picture = new Picture()
                    {
                        url = "ms-appx:///assets/SplashScreen.scale-200.png"
                    }
                },
                new MLItemHomeFeature()
                {
                    title = "Outro produto com descrição longa",
                    price = 11159.99,
                    picture = new Picture()
                    {
                        url = "ms-appx:///assets/Meu MercadoLivre Logo.png"
                    }
                },
            };

            Categories = new List<MLCategorySearchResult>()
            {
                new MLCategorySearchResult()
                {
                    id ="123",
                    name = "Acessórios para veículos"
                },
                new MLCategorySearchResult()
                {
                    id ="123",
                    name = "Agro, Indústria e Comércio"
                },
                new MLCategorySearchResult()
                {
                    id ="123",
                    name = "Animais"
                },
                new MLCategorySearchResult()
                {
                    id ="123",
                    name = "Antiguidades"
                }
            };
        }

        public IReadOnlyCollection<MLItemHomeFeature> Items { get; set; }
        public IReadOnlyCollection<MLCategorySearchResult> Categories { get; set; }
    }
}

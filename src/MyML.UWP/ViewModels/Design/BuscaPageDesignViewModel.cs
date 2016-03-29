
using MyML.UWP.Models;
using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.ViewModels.Design
{
    public class BuscaPageDesignViewModel
    {
        public BuscaPageDesignViewModel()
        {
            SellerInfo = new MLUserInfoSearchResult()
            {
                address = new Address()
                {
                    city = "Sao Paulo",
                    state = "São Paulo"
                },
                seller_reputation = new SellerReputation()
                {
                    power_seller_status = "platinum",
                    level_id = "5_green:"
                }
            };
            var options = new List<Option>();
            options.Add(new Option()
            {
                name = "Normal",
                list_cost = 16.99,
                cost = 16.99,
                estimated_delivery = new EstimatedDelivery() { date = DateTimeOffset.Parse("2015-07-02T00:00:00.000-03:00") }
            });
            ShippingInfo = new ShippingCost()
            {
                destination = new Destination()
                {
                    city = new City() { id = "Ata", name = "Araçatuba" },
                    country = new Country() { id = "HUE", name = "Brasil" },
                    extended_attributes = new ExtendedAttributes()
                    {
                        address = "Rua Doutor Álvaro Afonso do Nascimento, 146",
                        city_name = "Araçatuba",
                        neighborhood = "Jardim Presidente"
                    },
                    state = new State() { id = "SP", name = "São Paulo" },
                    zip_code = "16072-530",

                },
                options = options
            };
            FormaPagamento = "Em até 12x no cartão";

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

            FormasPagamentoAceitas = new List<PaymentMethod>();
            FormasPagamentoAceitas.Add(new PaymentMethod()
            {
                name = "visa",
                thumbnail = "ms-appx:///Assets/payments/hipercard.gif"
            });
            FormasPagamentoAceitas.Add(new PaymentMethod()
            {
                name = "visa",
                thumbnail = "ms-appx:///Assets/payments/hipercard.gif"
            });
            FormasPagamentoAceitas.Add(new PaymentMethod()
            {
                name = "visa",
                thumbnail = "ms-appx:///Assets/payments/hipercard.gif"
            });
            FormasPagamentoAceitas.Add(new PaymentMethod()
            {
                name = "visa",
                thumbnail = "ms-appx:///Assets/payments/hipercard.gif"
            });



            Items = new MLMyItemsSearchResult();
            Items.results_graph = new List<Item>();
            for (int i = 0; i < 10; i++)
            {
                Items.results_graph.Add(new Item()
                {
                    accepts_mercadopago = true,
                    available_quantity = 1,
                    address = new Address()
                    {
                        address = "Endereco que sobe e desce e nunca aparece",
                        city = "Araçatuba",
                        city_name = "Araçatuba",
                        state = "SP",
                        state_name = "SP",
                        zip_code = "1604444"
                    },
                    buying_mode = "",
                    condition = "new",
                    differential_pricing = new DifferentialPricing()
                    {

                    },
                    original_price = 500,
                    price = 500,
                    title = "Item de produto a venda",
                    seller_address = new SellerAddress()
                    {

                    },
                    thumbnail = "ms-appx:///assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg"

                });
            }
            //Items.results = new List
            //for (int i = 0; i < 10; i++)
            //{
            //    double? originalPrice = null;
            //    if (i % 2 == 0)
            //        originalPrice = new Random(100).NextDouble();
            //    else
            //        originalPrice = null;


            //    Installments parcelas = null;
            //    if (i % 2 == 0)
            //        parcelas = new Installments() { amount = 25, quantity = 12 };
            //    else
            //        parcelas = null;

            //    Items.Add(new UWP.Models.Mercadolivre.Item()
            //    {
            //        price = 500,
            //        title = "Produto com titulo muito grande pra testar a quebra de linha",
            //        thumbnail = "ms-appx:///Assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg",
            //        accepts_mercadopago = i % 2 == 0 ? true : false,
            //        original_price = originalPrice,
            //        installments = parcelas
            //    });
            //}





            QuestionsList = new ProductQuestion();
            QuestionsList.questions = new List<ProductQuestionContent>();
            for (int i = 0; i < 50; i++)
            {
                QuestionsList.questions.Add(new ProductQuestionContent()
                {
                    text = "Minha pergunta é com um texto muito grande pra ver como fica a quebra de linha em uma tela pequena.",
                    answer = new Answer() { text = "Minha resposta não é tao grande assim" }
                });
            }

            IsProductSelected = false;

            Questions = new List<QuestionGroup>();
            Questions.Add(new QuestionGroup()
            {
                Produto = new Item() { title = "Produto da pergunta", thumbnail = "ms-appx:///Assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg", price = 500, subtitle = "subtitulo" },
                Perguntas = new System.Collections.ObjectModel.ObservableCollection<ProductQuestionContent>(
                    new[] {
                            new ProductQuestionContent()
                                {
                                    answer = new Answer()   {
                                                            status = "A", text = "Respondida"
                                                        },
                                    text = "Minha pergunta vai aqui"
                                },
                            new ProductQuestionContent()
                                {
                                    answer = null,
                                    text = "Minha pergunta sem resposta vai aqui"
                                }
                         }
                   )
            });
            Questions.Add(new QuestionGroup()
            {
                Produto = new Item() { title = "Produto da pergunta 2", thumbnail = "ms-appx:///Assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg", price = 500, subtitle = "subtitulo" },
                Perguntas = new System.Collections.ObjectModel.ObservableCollection<ProductQuestionContent>(
                    new[] {
                            new ProductQuestionContent()
                                {
                                    answer = new Answer()   {
                                                            status = "A", text = "Respondida 2"
                                                        },
                                    text = "Minha pergunta 2 vai aqui"
                                },
                            new ProductQuestionContent()
                                {
                                    answer = null,
                                    text = "Minha pergunta 2 sem resposta vai aqui"
                                }
                         }
                   )
            });

            //Cria uma nova Order
            Func<MLOrderInfo> criarOrder = () =>
           {
               var result = new MLOrderInfo()
               {
                   status = "paid",
                   total_amount = 1500,
                   order_items = new List<OrderItem>(new OrderItem[]
                        {
                            new OrderItem()
                            {
                                item = new Item()
                                {
                                     thumbnail = "ms-appx:///Assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg",
                                     title = "Produto adicionado view DesignTime",
                                     price = 500
                                },
                                quantity = 1,
                                unit_price = 250,
                                sale_fee = 25
                            }
                        }),
                   buyer = new Buyer()
                   {
                       first_name = "Alexandre Dias",
                       last_name = "Simoes",
                       billing_info = new BillingInfo()
                       {
                           doc_number = "11111111111",
                           doc_type = "card"
                       }
                   },
                   date_created = DateTime.Now.ToString(),
                   status_detail = new StatusDetail()
                   {

                   }
               };
               return result;
           };
            Orders = new MLOrder()
            {
                results = new List<MLOrderInfo>(new MLOrderInfo[]
                {
                    criarOrder(),
                    criarOrder(),
                    criarOrder(),
                    criarOrder(),
                    criarOrder()
                })
            };

            //Detalhes da compra
            OrderInfo = new MLOrderInfo()
            {
                feedback = new Feedback()
                {
                    purchase = new Purchase()
                    {
                        message = "Apenas um teste",
                        rating = "positive"
                    }
                },

                status = "payment_required", //Mude para paid, para exibir os controles na tela CompraDetalhePage
                total_amount = 1500,
                order_items = new List<OrderItem>(new OrderItem[]
                         {
                            new OrderItem()
                            {
                                item = new Item()
                                {
                                     thumbnail = "ms-appx:///Assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg",
                                     title = "Produto adicionado view DesignTime",
                                     price = 500
                                },
                                quantity = 1,
                                unit_price = 250,
                                sale_fee = 25
                            }
                         }),
                buyer = new Buyer()
                {
                    first_name = "Alexandre Dias",
                    last_name = "Simoes",
                    billing_info = new BillingInfo()
                    {
                        doc_number = "11111111111",
                        doc_type = "card"
                    },
                    alternative_phone = new AlternativePhone()
                    {
                        area_code = "18",
                        number = "99999999"
                    },
                    phone = new Phone()
                    {
                        area_code = "18",
                        number = "99999999"
                    },
                    email = "alexandre.dias.simoes@outlook.com",
                    nickname = "alexandredsimoes"
                },
                seller = new Seller()
                {
                    email = "alexandre.dias.simoes@outlook",
                    first_name = "Alexandre Dias",
                    last_name = "Simoes",
                    nickname = "alexandresimoes",
                    power_seller_status = "platinum",
                    alternative_phone = new AlternativePhone2()
                    {
                        area_code = "18",
                        number = "99999999"
                    },
                    phone = new Phone2()
                    {
                        area_code = "18",
                        number = "99999999"
                    }
                },
                date_created = DateTime.Now.ToString(),
                status_detail = new StatusDetail()
                {

                },
                payments = new List<Payment>(new Payment[]
                {
                    new Payment()
                    {
                        date_created = DateTime.Now.ToString(),
                        id= 1786019122,
                        order_id= 1051132291,
                        payer_id= 41654723,
                        card_id= 157717439,
                        reason= "Cartão Microsoft Points Xbox Live Brasil De R$100 Imediato",
                        payment_method_id= "visa",
                        currency_id= "BRL",
                        installments= 1,
                        issuer_id= "25",
                        atm_transfer_reference= new AtmTransferReference()  {
                                                                                company_id= null,
                                                                                transaction_id= null
                                                                            },
                        coupon_id= null,
                        activation_uri= null,
                        operation_type= "regular_payment",
                        payment_type= "credit_card",
                        status= "approved",
                        status_code= "0",
                        status_detail= "accredited",
                        transaction_amount= 95.99,
                        shipping_cost= 0,
                        coupon_amount= 0,
                        overpaid_amount= 0,
                        total_paid_amount= 95.99,
                        date_last_modified= "2016-01-18T00=01=55.000-04=00"
                    }
                }),
                shipping = new Shipping()
                {
                    id = 21515717518,
                    site_id = "MLB",
                    shipment_type = "custom_shipping",
                    mode = "custom",
                    shipping_mode = "custom",
                    status = "pending",
                    shipping_items = new List<ShippingItem>(new ShippingItem[]
                    {
                    new ShippingItem(){
                        id= "MLB715217194",
                        description= "Cartão Microsoft Points Xbox Live Brasil De R$100 Imediato",
                        quantity= 1,
                        dimensions= null
                        }
                      }),
                    shipping_option = new ShippingOption()
                    {
                        id = null,
                        name = "add_shipping_cost",
                        currency_id = "BRL",
                        list_cost = 0,
                        cost = 0,
                        speed = null
                    },
                    currency_id = "BRL",
                    receiver_address = new ReceiverAddress()
                    {
                        id = 157705938,
                        address_line = "Rua Procópio Ferreira 521",
                        street_name = "Rua Procópio Ferreira",
                        street_number = "521",
                        comment = null,
                        zip_code = "16072580",
                        city = new City2()
                        {
                            id = "BR-SP-71",
                            name = "Araçatuba"
                        },
                        state = new State2()
                        {
                            id = "BR-SP",
                            name = "São Paulo"
                        },
                        country = new Country2()
                        {
                            id = "BR",
                            name = "Brasil"
                        },
                        neighborhood = new Neighborhood2()
                        {
                            id = null,
                            name = "Presidente"
                        },
                        municipality = new Municipality2()
                        {
                            id = null,
                            name = null
                        },
                        latitude = null,
                        longitude = null,
                        geolocation_type = null,
                        agency = null,
                        is_valid_for_carrier = null,
                        receiver_name = "Luzia dos Santos Ferreira",
                        receiver_phone = "(18) 3304-1107"
                    }
                }
            };

            Bookmarks = new List<MLBookmarkItem>();
            for (int i = 0; i < 10; i++)
            {
                Bookmarks.Add(new MLBookmarkItem()
                {
                    item_id = "",
                    ItemInfo = new Item()
                    {
                        title = "Produto Marcado como bookmark",
                        price = 1.99,
                        thumbnail = "ms-appx:///Assets/memoria-kingston-1gb-ddr2-800mhz-kvr800d2n61g-15190-MLB20098034628_052014-F.jpg"
                    }
                });
            }

            SelectedItem = new Item()
            {
                title = "Produto via designViewModel para compra",
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
                available_quantity = 222,
                shipping = new Shipping()
                {
                    free_shipping = false,
                    free_methods = new List<FreeMethod>(new FreeMethod[]
                    {

                        new FreeMethod()
                        {
                            id = 1,
                            rule = new Rule()
                            {
                               free_mode = "exclude_regions",
                               value = new List<string>(new [] {"BR-NO", "BR-NE"})
                            }
                        }
                    })
                }
            };

            SelectedProduct = SelectedItem;

            QuestionInfo = new ProductQuestionContent()
            {
                Item = SelectedItem,
                text = "Essa é minha pergunta",
                date_created = DateTime.Now,
                status = "active"
            };
        }

        public IList<AvailableFilter> Filters { get; set; } = new List<AvailableFilter>(new AvailableFilter[]
        {

            new AvailableFilter()
                {
                    id = "lojas",
                    name = "Lojas oficiais",
                    type = "text",
                    values = new System.Collections.ObjectModel.ObservableCollection<Value2>(new Value2[]
                        {
                            new Value2() { id = "1", name = "Todos"},
                            new Value2() { id = "1", name = "Loja 1"},
                            new Value2() { id = "1", name = "Loja 2"}
                        })
                },
            new AvailableFilter()
                {
                    id = "descontos",
                    name = "Descontos",
                    type = "text",
                    values = new System.Collections.ObjectModel.ObservableCollection<Value2>(new Value2[]
                        {
                            new Value2() { id = "1", name = "Mais 10%"},
                            new Value2() { id = "1", name = "Mais 20%"},
                            new Value2() { id = "1", name = "Acima de 40%"}
                        })
                },
            new AvailableFilter()
                {
                    id = "mp",
                    name = "Aceita mercado pago",
                    type = "boolean",
                    values = new System.Collections.ObjectModel.ObservableCollection<Value2>(new Value2[]
                        {                            
                            new Value2() { id = "yes", name = "Com mercado pago"}
                        })
                },
            new AvailableFilter()
                {
                    id = "mp",
                    name = "Com imagens",
                    type = "boolean",
                    values = new System.Collections.ObjectModel.ObservableCollection<Value2>(new Value2[]
                        {
                            new Value2() { id = "yes", name = "Items com imagem"}
                        })
                }
        });
        public ProductQuestionContent QuestionInfo { get; set; }

        public IList<QuestionGroup> Questions
        {
            get; set;
        }
        public MLMyItemsSearchResult Items
        {
            get;
            set;
        }

        public IList<MLBookmarkItem> Bookmarks { get; set; }
        public Item Item { get; set; }

        public bool IsProductSelected { get; set; }

        public IList<PaymentMethod> FormasPagamentoAceitas { get; set; }

        public ProductQuestion QuestionsList { get; set; }

        public string FormaPagamento { get; set; }

        public ShippingCost ShippingInfo { get; set; }

        public MLUserInfoSearchResult SellerInfo
        {
            get; set;
        }

        public MLOrder Orders { get; set; }
        public MLOrderInfo OrderInfo { get; private set; }
        public Item SelectedItem { get; set; }
        public Item SelectedProduct { get; set; }
    }
}

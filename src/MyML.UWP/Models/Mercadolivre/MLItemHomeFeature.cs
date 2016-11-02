namespace MyML.UWP.Models.Mercadolivre
{
    public class MLItemHomeFeature
    {
        public string item_id { get; set; }
        public string title { get; set; }
        public string listing_type_id { get; set; }
        public double? price { get; set; }
        public double? original_price { get; set; }
        public string currency_id { get; set; }
        public string permalink { get; set; }
        public Picture picture { get; set; }
        public bool? accepts_mercado_pago { get; set; }
        public string category_id { get; set; }
        public string pool_id { get; set; }
        public double? seller_id { get; set; }
        public Shipping shipping { get; set; }
    }
}
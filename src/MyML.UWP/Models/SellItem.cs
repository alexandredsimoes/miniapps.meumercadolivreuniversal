using GalaSoft.MvvmLight;
using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MyML.UWP.Models
{
    public class SellItem : ObservableObject
    {
        public SellItem()
        {
            Images = new ObservableCollection<ProductImage>();
        }
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { Set(() => Title, ref _Title, value); }
        }

        private double? _ProductValue;
        public double? ProductValue
        {
            get { return _ProductValue; }
            set { Set(() => ProductValue, ref _ProductValue, value); }
        }

        private string _ProductDescription;
        public string ProductDescription
        {
            get { return _ProductDescription; }
            set { Set(() => ProductDescription, ref _ProductDescription, value); }
        }

        private int? _Quantity;
        public int? Quantity
        {
            get { return _Quantity; }
            set { Set(() => Quantity, ref _Quantity, value); }
        }


        private ObservableCollection<ProductImage> _Images;
        public ObservableCollection<ProductImage> Images
        {
            get { return _Images; }
            set { Set(() => Images, ref _Images, value); }
        }

        private bool? _IsNew;
        public bool? IsNew
        {
            get { return _IsNew; }
            set { Set(() => IsNew, ref _IsNew, value); }
        }

        private MLListType _ListType;
        public MLListType ListType
        {
            get { return _ListType; }
            set { Set(() => ListType, ref _ListType, value); }
        }

        private string _BuyingMode;
        public string BuyingMode
        {
            get { return _BuyingMode; }
            set { Set(() => BuyingMode, ref _BuyingMode, value); }
        }

        private string _ProductCategory;

        public string ProductCategory
        {
            get { return _ProductCategory; }
            set { Set(() => ProductCategory, ref _ProductCategory, value); }
        }


    }
    public class ProductImage : ObservableObject
    {
        private string _LocalPath;

        public string LocalPath
        {
            get { return _LocalPath; }
            set { Set(() => LocalPath, ref _LocalPath, value); }
        }

        private BitmapSource _Source;
        public BitmapSource Source
        {
            get { return _Source; }
            set { Set(() => Source, ref _Source, value); }
        }

    }
}

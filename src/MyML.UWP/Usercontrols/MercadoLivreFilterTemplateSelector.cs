using MyML.UWP.Models.Mercadolivre;
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
using Windows.UI.Xaml.Navigation;

namespace MyML.UWP.Usercontrols
{
    public class MercadoLivreFilterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BooleanDataTemplate { get; set; }
        public DataTemplate TextDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var dataItem = item as AvailableFilter;
            if (dataItem.type == "text" || dataItem.type == "range")
                return TextDataTemplate;
            else if(dataItem.type == "boolean")
                return BooleanDataTemplate;

            return null;
        }       
    }
}

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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyML.UWP.Usercontrols
{
    public sealed partial class ActionButtonControl : UserControl
    {
        public ActionButtonControl()
        {
            this.InitializeComponent();
        }



        public string Icone
        {
            get { return (string)GetValue(IconeProperty); }
            set { SetValue(IconeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconeProperty =
            DependencyProperty.Register("Icone", typeof(string), typeof(ActionButtonControl), new PropertyMetadata(0));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ActionButtonControl), new PropertyMetadata(0));
    }
}

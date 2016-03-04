using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyML.UWP.Behaviors
{
    public static class WebBrowserHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof(string), typeof(WebBrowserHelper), new PropertyMetadata(null, OnHtmlChanged));

        public static readonly DependencyProperty UrlProperty = DependencyProperty.RegisterAttached(
            "Url", typeof(string), typeof(WebBrowserHelper), new PropertyMetadata(null, OnUrlChanged));

        public static string GetHtml(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebView;

            if (browser == null)
                return;

            var html = e.NewValue.ToString();

            browser.NavigateToString(html);
        }




        public static string GetUrl(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(UrlProperty);
        }

        public static void SetUrl(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(UrlProperty, value);
        }

        private static void OnUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebView;

            if (browser == null)
                return;

            var url = e.NewValue.ToString();

            browser.Navigate(new Uri(url));
        }
    }
}

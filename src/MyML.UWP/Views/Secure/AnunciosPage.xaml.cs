﻿using System;
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
using Template10.Services.SerializationService;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyML.UWP.Views.Secure
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnunciosPage : Page
    {
        public AnunciosPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            var parameter = e.Parameter as string;
            if (parameter != null)
            {
                var obj = SerializationService.Json.Deserialize(parameter);
                var s = obj as string;
                if (s == "Active")
                    PivotMenu.SelectedIndex = 0;
                if (s == "Paused")
                    PivotMenu.SelectedIndex = 1;
                if (s == "Finalized")
                    PivotMenu.SelectedIndex = 2;
            }
        }
    }
}

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MyML.UWP.Aggregattor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.ViewModels
{
    public class OrdernarBuscaPageViewModel
    {
        public OrdernarBuscaPageViewModel()
        {
            //Messenger.Default.Register<MessengerFilterDetails>(this, SetMessengerFilters);
            //SaveFilter = new RelayCommand(SaveFilterExecute);

            //SelectedSortFilter = new AvailableSort() { id = "-1", name = "Mais relevantes" };

            //SelectOrder = new RelayCommand<object>((obj) =>
            //{
            //    var item = obj as AvailableSort;

            //    if (item != null)
            //    {
            //        SelectedSortFilter = item;
            //        //await new MessageDialog(((AvailableSort)item).name).ShowAsync();
            //    }
            //    else
            //    {

            //    }
            //    //var index = (int)obj;
            //    //var sort = Sorts.ElementAt(index);
            //});

            //AddFilter = new RelayCommand<object>(AddFilterExecute);
        }
    }
}

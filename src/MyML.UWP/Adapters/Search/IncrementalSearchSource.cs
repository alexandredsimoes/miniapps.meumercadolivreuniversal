using GalaSoft.MvvmLight.Threading;
using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyML.UWP.Adapters
{
    public class IncrementalSearchSource<T, K> : ObservableCollection<K>, ISupportIncrementalLoading
        where T : IPagedSource<K>, new()
    {
        private string CategoryName { get; set; }
        private bool SearchByName { get; set; }
        private int VirtualCount { get; set; }
        private int CurrentPage { get; set; }
        private int PageSize { get; set; }
        private IPagedSource<K> Source { get; set; }

        public IncrementalSearchSource(int pageIndex, int pageSize, string query = null, bool searchByProductName = false)
        {
            this.Source = new T();

            //((OrdersDataSource)this.Source).GetFilters += IncrementalSource_GetFilters;

            this.CurrentPage = pageIndex;
            this.PageSize = pageSize;
            this.CategoryName = query;
            this.VirtualCount = int.MaxValue;
            this.SearchByName = searchByProductName;
        }

        //void IncrementalSource_GetFilters(IList<Model.Mercadolivre.AvailableFilter> obj)
        //{
        //    OnFiltersAvailable(obj);
        //}


        #region ISupportIncrementalLoading

        public bool HasMoreItems
        {
            get { return this.VirtualCount > this.CurrentPage * this.PageSize; }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            //return AsyncInfo.Run((c) => { LoadMoreItemsAsync(c, count)});
            return AsyncInfo.Run(
                async (c) =>
                {
                    WindowWrapper.Current().Dispatcher.Dispatch(() =>
                    {
                        OnLoadMoreItemsStarted();
                    });
                    //await DispatcherHelper.RunAsync(() =>
                    //{
                    //    OnLoadMoreItemsStarted();
                    //});

                    IPagedResponse<K> result = await Source.GetPage(CategoryName, CurrentPage++, PageSize, SearchByName);
                    this.VirtualCount = result == null ? 0 : result.VirtualCount;


                    if (result == null)
                    {

                        OnLoadMoreItemsCompleted(result.Paging);//terminamos por aqui
                        return new LoadMoreItemsResult() { Count = 0 };
                    }


                    WindowWrapper.Current().Dispatcher.Dispatch(() =>
                    {
                        foreach (K item in result.Items)
                            Add(item);


                        OnLoadMoreItemsCompleted(result.Paging);
                        OnFiltersAvailable(result.Filters, result.Sorts);
                    });

                    return new LoadMoreItemsResult() { Count = (uint)result.Items.Count() };
                });
        }

        #endregion



        public event Action<IList<AvailableFilter>, IList<AvailableSort>> FiltersAvailable;
        //public event Action<IList<AvailableSort>> SortsAvailable;
        public event Action LoadMoreItemsStarted;
        public event Action<Paging> LoadMoreItemsCompleted;

        protected virtual void OnLoadMoreItemsStarted()
        {
            var handler = LoadMoreItemsStarted;
            if (handler != null) handler();
        }

        protected virtual void OnLoadMoreItemsCompleted(Paging paging)
        {            
            var handler = LoadMoreItemsCompleted;
            if (handler != null) handler(paging);
        }

        protected virtual void OnFiltersAvailable(IList<AvailableFilter> filters, IList<AvailableSort> sorts)
        {
            var handler = FiltersAvailable;
            if (handler != null) handler(filters, sorts);
        }
    }

}

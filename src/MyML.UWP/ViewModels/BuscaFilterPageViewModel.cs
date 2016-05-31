using GalaSoft.MvvmLight.Messaging;
using MyML.UWP.Aggregattor;
using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using Template10.Services.NavigationService;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Popups;
using System.Diagnostics;

namespace MyML.UWP.ViewModels
{
    public class BuscaFilterPageViewModel : ViewModelBase
    {

        public BuscaFilterPageViewModel()
        {
            Messenger.Default.Register<MessengerFilterDetails>(this, SetMessengerFilters);
            SaveFilter = new RelayCommand(SaveFilterExecute);

            SelectedSortFilter = new AvailableSort() { id = "-1", name = "Mais relevantes" };

            SelectOrder = new RelayCommand<object>((obj) =>
           {
               var item = obj as AvailableSort;

               if (item != null)
               {
                   SelectedSortFilter = item;
                   //await new MessageDialog(((AvailableSort)item).name).ShowAsync();
               }
               else
               {

               }
               //var index = (int)obj;
               //var sort = Sorts.ElementAt(index);
           });

            AddFilter = new RelayCommand<object>(AddFilterExecute);

            SelectSortItem = new RelayCommand<object>((obj) =>
            {
                var item = obj as AvailableSort;

                if (item == null) return;
                SelectedSorts.Clear();
                SelectedSorts.Add(item);
            });
        }

        private void AddFilterExecute(object _obj)
        {
            var obj = _obj as Value2;

            if (obj is Value2)
            {
                AvailableFilter filterAdd = new AvailableFilter();
                if (SelectedFilters.Any(c => c.id == obj.filter.id))
                {

                    var itemExists = SelectedFilters.FirstOrDefault(c => c.id == obj.filter.id);

                    Debug.WriteLine($"Removendo filtro -> {itemExists.id}={itemExists.values[0].id}");

                    SelectedFilters.Remove(itemExists);

                    if (obj.id != "-1")
                    {
                        filterAdd.id = obj.filter.id;
                        filterAdd.name = obj.filter.name;
                        filterAdd.type = obj.filter.type;
                        filterAdd.values.Add(obj);

                        Debug.WriteLine($"Filtro adicionado -> {filterAdd.id}={filterAdd.values[0].id}");
                        SelectedFilters.Add(filterAdd);
                    }
                }
                else
                {
                    if (obj.id != "-1")
                    {
                        filterAdd.id = obj.filter.id;
                        filterAdd.name = obj.filter.name;
                        filterAdd.type = obj.filter.type;
                        filterAdd.values.Add(obj);

                        Debug.WriteLine($"Filtro adicionado -> {filterAdd.id}={filterAdd.values[0].id}");
                        SelectedFilters.Add(filterAdd);
                    }
                }
            }
            else
            {
                //Nosso filtro boolean
                var objFilter = _obj as AvailableFilter;

                if (objFilter != null)
                {
                    if (SelectedFilters.Any(c => c.id == objFilter.id))
                    {
                        var filterExists = SelectedFilters.FirstOrDefault(c => c.id == objFilter.id);
                        if (filterExists != null)
                            SelectedFilters.Remove(filterExists);

                        SelectedFilters.Add(objFilter);
                    }
                    else
                    {
                        SelectedFilters.Add(objFilter);
                    }
                }
            }
        }

        private void SaveFilterExecute()
        {
            StringBuilder sb = new StringBuilder();
            //Processa os filtros
            Debug.WriteLine("Filtros selecionados");

            //Monta a string do filtro
            for (int i = 0; i < SelectedFilters.Count; i++)
            {
                sb.Append($"{SelectedFilters[i].id}={SelectedFilters[i].values[0].id}" + (i < SelectedFilters.Count - 1 ? "&" : ""));
            }

            //Monta a ordenação
            if (sb.Length > 0)
                sb.Append("&");
            for (int i = 0; i < SelectedSorts.Count; i++)
            {
                sb.Append($"sort={SelectedSorts[i].id}" + (i < SelectedSorts.Count - 1 ? "&" : ""));
            }

            //Notifica a página anterior com o filtro montado
            Messenger.Default.Send<MessengerFilterResult>(new MessengerFilterResult() { Result = sb.ToString(), SelectedFilters = this.SelectedFilters, SelectedSorts = this.SelectedSorts });
            sb.Clear();

            NavigationService.GoBack();
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            return Task.CompletedTask;
        }


        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Views.Shell.SetBusy(false);
            return Task.CompletedTask;
        }

        private void SetMessengerFilters(MessengerFilterDetails obj)
        {
            if (Filters != null) Filters.Clear();
            if (Sorts != null) Sorts.Clear();


            if (obj.Filters == null /*|| obj.Sorts == null*/) return;

            if (!GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                foreach (var item in obj.Filters)
                {
                    Filters.Add(item);
                }
                foreach (var item in obj.Sorts)
                {
                    Sorts.Add(item);
                }


                //Insere os valores default
                if (!Sorts.Any(c => c.id == "relevance"))
                    Sorts.Insert(0, new AvailableSort() { id = "relevance", name = "Mais relevantes" });


                foreach (var item in Filters.Where(c => c.type == "text" || c.type == "range"))
                {
                    if (!item.values.Any(c => c.id == "-1"))
                        item.values.Insert(0, new Value2()
                        {
                            id = "-1",
                            name = "Todos"
                        });
                }
            }
        }

        private IList<AvailableFilter> _Filters = new List<AvailableFilter>();
        public IList<AvailableFilter> Filters
        {
            get { return _Filters; }
            set { Set(() => Filters, ref _Filters, value); }
        }

        private IList<AvailableSort> _Sorts = new List<AvailableSort>();
        public IList<AvailableSort> Sorts
        {
            get { return _Sorts; }
            set { Set(() => Sorts, ref _Sorts, value); }
        }

        private AvailableSort _SelectedSortFilter;
        public AvailableSort SelectedSortFilter
        {
            get { return _SelectedSortFilter; }
            set { Set(() => SelectedSortFilter, ref _SelectedSortFilter, value); }
        }

        private IList<AvailableFilter> _SelectedFilters = new List<AvailableFilter>();
        public IList<AvailableFilter> SelectedFilters
        {
            get { return _SelectedFilters; }
            set { Set(() => SelectedFilters, ref _SelectedFilters, value); }
        }

        private IList<AvailableSort> _SelectedSorts = new List<AvailableSort>();
        public IList<AvailableSort> SelectedSorts
        {
            get { return _SelectedSorts; }
            set { Set(() => SelectedSorts, ref _SelectedSorts, value); }
        }



        public RelayCommand SaveFilter { get; private set; }
        public RelayCommand<object> SelectOrder { get; private set; }
        public RelayCommand<object> AddFilter { get; private set; }
        public RelayCommand<object> SelectSortItem { get; set; }

    }
}

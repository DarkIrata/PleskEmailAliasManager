using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IPUP.MVVM.Commands;
using IPUP.MVVM.Events;
using IPUP.MVVM.ViewModels;
using PleskEmailAliasManager.Data;

namespace PleskEmailAliasManager.ViewModels
{
    internal abstract class ListEventViewModelBase<T> : EventViewModelBase
    {
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        protected List<T> RawItems { get; set; } = new List<T>();

        private T? selectedItem;

        public T? SelectedItem
        {
            get => this.selectedItem;
            set
            {
                this.Set(ref this.selectedItem, value, nameof(this.SelectedItem));
                this.OnSelectedItemChanged();
            }
        }

        private bool sortAscending = true;

        public bool SortAscending
        {
            get => this.sortAscending;
            set => this.Set(ref this.sortAscending, value, nameof(this.SortAscending));
        }

        private string? filterText = string.Empty;

        public string? FilterText
        {
            get => this.filterText;
            set
            {
                this.Set(ref this.filterText, value, nameof(this.FilterText));
                this.RefreshItems();
                this.OnFilterTextChanged();
            }
        }

        public DelegateCommand ReloadItemsCommand { get; }

        public DelegateCommand FlipSortCommand { get; }

        public ListEventViewModelBase(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            this.ReloadItemsCommand = new DelegateCommand(async () => await this.ExecuteReload(), () => this.CanExecuteReload());
            this.FlipSortCommand = new DelegateCommand(() => this.ExecuteFlipSort(), () => this.CanExecuteFlipSort());

            Task.Run(async () => await this.ReloadItems());
        }

        public virtual void OnSelectedItemChanged()
        {
        }

        internal async Task<bool> ReloadItems()
        {
            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                this.Items.Clear();
                this.RawItems.Clear();
            });

            var response = await this.RequestItems();
            var successfully = response.Result?.Successfully ?? false; // Only when successfully
            if (successfully)
            {
                this.RawItems = response.Items;
            }

            this.RefreshItems();

            return successfully;
            // ToDo: Error Handling
        }

        internal async Task RefreshControls()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                this.ReloadItemsCommand?.RaiseCanExecuteChanged();
                this.FlipSortCommand?.RaiseCanExecuteChanged();
            });
        }

        protected void ExecuteFlipSort()
        {
            this.SortAscending = !this.SortAscending;
            this.RefreshItems();
        }

        private void RefreshItems()
        {
            var tempItems = this.GetSortedAndFilteredItems();
            this.FillItems(tempItems);
        }

        protected void FillItems(IEnumerable<T> items)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                this.Items.Clear();

                foreach (var item in items)
                {
                    this.Items.Add(item);
                }

                Task.Run(async () => await this.OnItemsRefreshed());
            });
        }

        internal virtual async Task OnItemsRefreshed()
        {
            await Task.CompletedTask;
        }

        internal abstract IEnumerable<T> GetSortedAndFilteredItems();

        internal virtual async Task ExecuteReload()
        {
            await this.ReloadItems();
        }

        internal virtual void OnFilterTextChanged()
        {
        }

        internal abstract Task<(ErrorResult Result, List<T> Items)> RequestItems();

        internal abstract bool CanExecuteReload();

        internal abstract bool CanExecuteFlipSort();
    }
}

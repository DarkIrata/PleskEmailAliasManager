using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IPUP.MVVM.Commands;
using IPUP.MVVM.Events;
using IPUP.MVVM.ViewModels;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Interfaces;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    internal abstract class PleskDataViewModel<TItem> : ListEventViewModelBase<TItem>
        where TItem : class, IDisplayNameCanFilter
    {
        internal PleskMailManager Provider { get; }

        public PleskDataViewModel(IEventAggregator eventAggregator, PleskMailManager provider)
            : base(eventAggregator)
        {
            this.Provider = provider;

            Task.Run(async () => await this.ReloadItems());
        }
        
        internal override IEnumerable<TItem> GetSortedAndFilteredItems()
        {
            var filteredItems = this.RawItems.Where(i => i.GetDisplayNameForFilter().Contains(this.FilterText!));
            if (!this.SortAscending)
            {
                return filteredItems.OrderByDescending(i => i.GetDisplayNameForFilter());
            }

            return filteredItems.OrderBy(i => i.GetDisplayNameForFilter());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IPUP.MVVM.Events;
using IPUP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Events;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    internal class DomainsViewModel : PleskDataViewModel<DomainData>
    {
        public DomainsViewModel(IEventAggregator eventAggregator, PleskMailManager provider)
            : base(eventAggregator, provider)
        {
        }

        public override async void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            await this.EventAggregator.PublishAsync(new SelectedDomainChangedEvent(this.SelectedItem));
        }

        internal override bool CanExecuteFlipSort() => true;

        internal override bool CanExecuteReload() => true;

        internal override async Task<(ErrorResult Result, List<DomainData> Items)> RequestItems() => await this.Provider.GetDomainsAsync();
    }
}

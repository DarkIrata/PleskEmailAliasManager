using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPUP.MVVM.Events;
using IPUP.MVVM.ViewModels;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    internal class AddressViewModel : EventViewModelBase
    {
        private readonly PleskMailManager pleskManager;

        public DomainsViewModel Domains { get; }

        public MailsViewModel Mails { get; }

        public AliasesViewModel Aliases { get; }

        public AddressViewModel(IEventAggregator eventAggregator, PleskMailManager pleskManager)
            : base(eventAggregator)
        {
            this.pleskManager = pleskManager;
            this.Domains = new DomainsViewModel(eventAggregator, pleskManager);
            this.Mails = new MailsViewModel(eventAggregator, pleskManager);
            this.Aliases = new AliasesViewModel(eventAggregator, pleskManager);
        }
    }
}

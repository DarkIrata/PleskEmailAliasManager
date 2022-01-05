using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPUP.MVVM.Events;
using PleskEmailAliasManager.Data;

namespace PleskEmailAliasManager.Events
{
    internal class SelectedDomainChangedEvent : IEventParams
    {
        internal DomainData? DomainData { get; }

        public bool IsHandled { get; set; }

        public SelectedDomainChangedEvent(DomainData? domainData)
        {
            this.DomainData = domainData;
        }
    }
}

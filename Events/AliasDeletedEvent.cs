using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPUP.MVVM.Events;

namespace PleskEmailAliasManager.Events
{
    internal class AliasDeletedEvent : IEventParams
    {
        public bool IsHandled { get; set; }

        public string Alias { get; }

        public AliasDeletedEvent(string alias)
        {
            this.Alias = alias;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPUP.MVVM.Events;
using PleskEmailAliasManager.Data;

namespace PleskEmailAliasManager.Events
{
    internal class RefreshAndReselectMailEvent : IEventParams
    {
        internal MailData MailData { get; }

        public bool IsHandled { get; set; }

        public RefreshAndReselectMailEvent(MailData mailData)
        {
            this.MailData = mailData;
        }
    }
}

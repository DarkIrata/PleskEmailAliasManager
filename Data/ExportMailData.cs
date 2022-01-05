using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleskEmailAliasManager.Data
{
    internal class ExportMailData
    {
        public string Mail { get; set; } = string.Empty;

        public List<string> Aliases { get; set; } = new List<string>();

        public ExportMailData(string mail, List<string>? aliases)
        {
            this.Mail = mail;
            this.Aliases = aliases ?? new List<string>();
        }
    }
}

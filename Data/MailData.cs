using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PleskEmailAliasManager.Interfaces;

namespace PleskEmailAliasManager.Data
{
    internal class MailData : IDisplayNameCanFilter
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public List<string>? Aliases { get; set; }

        public DomainData? DomainData { get; set; }

        public string NameWithDomain => $"{this.Name}@{this.DomainData?.Name}";

        public MailData(int? id, string? name, List<string>? alias = null, DomainData? domainData = null)
        {
            this.Id = id;
            this.Name = name;
            this.Aliases = alias;
            this.DomainData = domainData;
        }

        public string GetDisplayNameForFilter() => (this.Name ?? this.Id?.ToString()) ?? string.Empty;
    }
}

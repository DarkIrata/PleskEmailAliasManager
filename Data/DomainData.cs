using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PleskEmailAliasManager.Enums;
using PleskEmailAliasManager.Interfaces;

namespace PleskEmailAliasManager.Data
{
    internal class DomainData : IDisplayNameCanFilter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DomainType DomainType { get; set; }

        public DomainData(int id, string name, DomainType domainType)
        {
            this.Id = id;
            this.Name = name;
            this.DomainType = domainType;
        }

        public string GetDisplayNameForFilter() => this.Name ?? this.Id.ToString();
    }
}

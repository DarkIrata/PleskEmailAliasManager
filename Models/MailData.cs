using System.Collections.Generic;

namespace PleskEmailAliasManager.Models
{
    public class MailData
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public List<string> Aliases { get; set; }

        public DomainData DomainData{ get; set; }

        public string NameWithDomain => $"{this.Name}@{this.DomainData?.Name}";

        public MailData(int id, string name, List<string> alias = null, DomainData domainData = null)
        {
            this.Id = id;
            this.Name = name;
            this.Aliases = alias;
            this.DomainData = domainData;
        }
    }
}

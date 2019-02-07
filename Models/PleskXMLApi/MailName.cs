using System.Collections.Generic;
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("mailname")]
    public class MailName
    {
        [XmlElement("id")]
        public int? Id { get; set; }

        public bool ShouldSerializeId() { return this.Id.HasValue; }

        [XmlElement("name")]
        public string Name { get; set; }


        [XmlElement("alias")]
        public List<string> Aliases { get; set; }
    }
}

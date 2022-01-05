using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("filter")]
    public class Filter
    {
        [XmlElement("site-id")]
        public int? SiteId { get; set; }

        public bool ShouldSerializeSiteId() { return this.SiteId.HasValue; }

        [XmlElement("mailname")]
        public MailName? MailName { get; set; }
    }
}
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("data")]
    public class Data
    {
        [XmlElement("gen_info")]
        public GenInfo? GenInfo { get; set; }
    }
}

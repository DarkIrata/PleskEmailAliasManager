using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("data")]
    public class Data
    {
        [XmlElement("gen_info")]
        public GenInfo GenInfo { get; set; }
    }
}

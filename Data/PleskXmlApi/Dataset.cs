using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("dataset")]
    public class Dataset
    {
        [XmlElement("gen_info")]
        public GenInfo? GenInfo { get; set; }
    }
}
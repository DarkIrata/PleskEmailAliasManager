using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("dataset")]
    public class Dataset
    {
        [XmlElement("gen_info")]
        public GenInfo GenInfo { get; set; }
    }
}
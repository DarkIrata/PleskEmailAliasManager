using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("system")]
    public class System
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("errcode")]
        public int ErrorCode { get; set; }

        [XmlElement("errtext")]
        public string ErrorText { get; set; }
    }
}

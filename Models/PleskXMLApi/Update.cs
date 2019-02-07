using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    public class Update
    {
        [XmlElement("add")]
        public Add Add { get; set; }


        [XmlElement("remove")]
        public Remove Remove { get; set; }
    }
}

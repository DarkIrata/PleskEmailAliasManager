using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    public class Update
    {
        [XmlElement("add")]
        public Add? Add { get; set; }


        [XmlElement("remove")]
        public Remove? Remove { get; set; }
    }
}

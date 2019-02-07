using System.Collections.Generic;
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("get_info")]
    public class GetInfo
    {
        [XmlElement("filter")]
        public Filter Filter { get; set; }

        [XmlElement("aliases")]
        public Aliases Aliases { get; set; }

        [XmlElement("result")]
        public List<Result> Result { get; set; } = new List<Result>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("get")]
    public class Get
    {
        [XmlElement("filter")]
        public Filter? Filter { get; set; }

        [XmlElement("dataset")]
        public Dataset? Dataset { get; set; }

        [XmlElement("result")]
        public List<Result> Result { get; set; } = new List<Result>();
    }
}
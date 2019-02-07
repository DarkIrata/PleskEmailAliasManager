using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("webspace")]
    public class Webspace
    {
        [XmlElement("get")]
        public Get Get { get; set; }

        public static Webspace CreateMinimumGet() =>
            new Webspace()
            {
                Get = new Get()
                {
                    Filter = new Filter(),
                    Dataset = new Dataset()
                }
            };
    }
}

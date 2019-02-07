﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("gen_info")]
    public class GenInfo
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("result")]
        public List<Result> Result { get; set; } = new List<Result>();
    }
}

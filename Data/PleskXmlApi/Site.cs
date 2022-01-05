using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("site")]
    public class Site
    {
        [XmlElement("get")]
        public Get? Get { get; set; }

        public static Site CreateMinimumGet() => 
            new Site()
            {
                Get = new Get()
                {
                    Filter = new Filter(),
                    Dataset = new Dataset()
                }
            };
    }
}

using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("customer")]
    public class Customer
    {
        [XmlElement("get")]
        public Get Get { get; set; }

        public static Customer CreateMinimumGet() =>
            new Customer()
            {
                Get = new Get()
                {
                    Filter = new Filter(),
                    Dataset = new Dataset()
                }
            };
    }
}

using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("customer")]
    public class Customer
    {
        [XmlElement("get")]
        public Get? Get { get; set; }

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

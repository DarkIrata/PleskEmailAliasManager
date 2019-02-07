using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models.PleskXMLApi
{
    [XmlRoot("result")]
    public class Result
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("errcode")]
        public int ErrorCode { get; set; }

        [XmlElement("errtext")]
        public string ErrorText { get; set; }

        [XmlElement("filter-id")]
        public int FilterId { get; set; }

        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("data")]
        public Data Data { get; set; }

        [XmlElement("mailname")]
        public MailName MailName { get; set; }
    }
}

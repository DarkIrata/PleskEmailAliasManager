using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("mail")]
    public class Mail
    {
        [XmlElement("get_info")]
        public GetInfo? GetInfo { get; set; }

        [XmlElement("update")]
        public Update? Update { get; set; }

        public static Mail CreateMinimumGet(int siteId)
        {
            var mail = new Mail()
            {
                GetInfo = new GetInfo()
                {
                    Filter = new Filter()
                }
            };

            mail.GetInfo.Filter.SiteId = siteId;

            return mail;
        }
    }
}


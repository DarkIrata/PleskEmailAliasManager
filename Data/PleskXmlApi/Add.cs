using System.Collections.Generic;
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    public class Add
    {
        [XmlElement("filter")]
        public Filter? Filter { get; set; }

        [XmlElement("result")]
        public List<Result> Result { get; set; } = new List<Result>();

        public Add(int siteId, string mainMailName)
        {
            this.Filter = new Filter()
            {
                SiteId = siteId,
                MailName = new MailName()
                {
                    Name = mainMailName
                }
            };
        }

        public Add()
        {
        }
    }
}

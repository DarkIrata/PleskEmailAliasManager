using System.Xml.Serialization;

namespace PleskEmailAliasManager.Models
{
    public class LoginDetails
    {
        public string Server { get; set; } = "http://HOST.TLD:8443";

        public string Username { get; set; }

        [XmlIgnore]
        public string Password { get; set; }

        [XmlIgnore]
        public bool SaveLoginDetails { get; set; } = true;

        public LoginDetails()
        {
        }
    }
}
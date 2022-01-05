using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PleskEmailAliasManager.Data
{
    internal class LoginDetails
    {
        public string? Host { get; set; } = "http://HOST.TLD:8443";

        public string? Username { get; set; }

        [JsonIgnore]
        public string? Password { get; set; }

        public LoginDetails(string? host, string? username, string? password)
        {
            Host = host;
            Username = username;
            Password = password;
        }
        public LoginDetails()
        {
        }
    }
}

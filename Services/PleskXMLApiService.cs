using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PleskEmailAliasManager.Models;
using PleskEmailAliasManager.Models.PleskXMLApi;

namespace PleskEmailAliasManager.Services
{
    public class PleskXMLApiService
    {
        private const string agentUrl = "/enterprise/control/agent.php";

        private readonly LoginDetails loginDetails;
        private readonly HttpClient client;

        private string ApiUrl => this.loginDetails.Server + agentUrl;

        public PleskXMLApiService(LoginDetails loginDetails)
        {
            this.loginDetails = loginDetails ??
                    throw new ArgumentNullException(nameof(loginDetails));

            this.client = new HttpClient();
            this.client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
            this.client.DefaultRequestHeaders.Add("HTTP_AUTH_LOGIN", this.loginDetails.Username);
            this.client.DefaultRequestHeaders.Add("HTTP_AUTH_PASSWD", this.loginDetails.Password);
        }


        public async Task<Packet> RequestAsync(Packet packet)
        {
            //var content = new Content
            var postResponse = await this.client.PostAsync(this.ApiUrl, new StringContent(packet.Serialize())).ConfigureAwait(false);
            var rawResponse = await postResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            return Packet.Deserialize(rawResponse);
        }
    }
}

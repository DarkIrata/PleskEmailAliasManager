using System;
using System.Net.Http;
using System.Threading.Tasks;
using PleskEmailAliasManager.Data;
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

        public async Task<(ErrorResult ErrorResult, Packet Packet)> RequestAsync(Packet packet)
        {
            var packetData = packet.Serialize();
            var content = new StringContent(packetData);

            try
            {
                var postResponse = await this.client.PostAsync(this.ApiUrl, content).ConfigureAwait(false);
                var rawResponse = await postResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                return (ErrorResult.Success(), Packet.Deserialize(rawResponse));
            }
            catch (Exception ex)
            {
                return (new ErrorResult(ErrorCode.InternalError, ex.Message, ex), null);
            }
        }
    }
}

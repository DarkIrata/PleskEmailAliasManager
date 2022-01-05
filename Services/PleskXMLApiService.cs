using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Data.PleskXmlApi;
using PleskEmailAliasManager.Enums;

namespace PleskEmailAliasManager.Services
{
    internal class PleskXMLApiService
    {
        private const string AgentUrl = "/enterprise/control/agent.php";

        private readonly LoginDetails loginDetails;
        private readonly HttpClient httpClient;
        private readonly string resultErrorCodeXmlName = string.Empty;
        private readonly string resultErrorTextXmlName = string.Empty;

        private string ApiUrl => (this.loginDetails?.Host ?? string.Empty) + AgentUrl;

        public PleskXMLApiService(LoginDetails loginDetails)
        {
            this.loginDetails = loginDetails;

            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            this.httpClient.DefaultRequestHeaders.Add("HTTP_AUTH_LOGIN", this.loginDetails.Username);
            this.httpClient.DefaultRequestHeaders.Add("HTTP_AUTH_PASSWD", this.loginDetails.Password);
            this.httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            var resultType = typeof(Result);
            this.resultErrorCodeXmlName = this.GetXmlElementAttributeNameFromProperty(resultType, nameof(Result.ErrorCode));
            this.resultErrorTextXmlName = this.GetXmlElementAttributeNameFromProperty(resultType, nameof(Result.ErrorText));
        }

        private string GetXmlElementAttributeNameFromProperty(Type type, string propertyName)
        {
            // This can crash and is intended to if something was changed. This or have a magic string someone can forgot to update...
            // Not the finest work, but crashing here is a real edge case. 
            var xmlElementAttribute = type.GetProperty(propertyName)!.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(XmlElementAttribute));
            return xmlElementAttribute!.ConstructorArguments[0]!.Value!.ToString()!;
        }

        public async Task<(ErrorResult ErrorResult, Packet? Packet)> RequestAsync(Packet packet)
        {
            StringContent? content = null;

            try
            {
                var packetData = packet.Serialize();
                content = new StringContent(packetData);
            }
            catch (Exception ex)
            {
                return (new ErrorResult(ErrorCode.FailedSerializePackage, ex), null);
            }

            try
            {
                var postResponse = await this.httpClient.PostAsync(this.ApiUrl, content).ConfigureAwait(false);
                var rawResponse = await postResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = Packet.Deserialize(rawResponse);

                if (result?.System != null && result.System?.ErrorCode != 0)
                {
                    return (new ErrorResult(ErrorCode.RequestFailed, result.System?.ErrorText, null), result);
                }
                else if (rawResponse.Contains(nameof(Result), StringComparison.OrdinalIgnoreCase))
                {
                    bool resultHasError(XElement resultElement)
                    {
                        var errorCodeElement = resultElement.Element(this.resultErrorCodeXmlName);
                        if (errorCodeElement != null)
                        {
                            return errorCodeElement.Value != "0";
                        }

                        return false;
                    }

                    var xdoc = XDocument.Parse(rawResponse.ToLower());
                    var firstResultInPaket = xdoc.Descendants(nameof(Result).ToLower()).FirstOrDefault(r => resultHasError(r));
                    if (firstResultInPaket != null)
                    {
                        return (new ErrorResult(ErrorCode.RequestFailed, firstResultInPaket.Element(this.resultErrorTextXmlName)?.Value, null), result);
                    }
                }

                return (ErrorResult.Success, result);
            }
            catch (Exception ex)
            {
                return (new ErrorResult(ErrorCode.InternalError, ex), null);
            }
        }
    }
}

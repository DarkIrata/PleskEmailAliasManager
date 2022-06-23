using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Data.PleskXmlApi;
using PleskEmailAliasManager.Enums;

namespace PleskEmailAliasManager.Services
{
    internal class PleskXMLApiService : IDisposable
    {
        private const string AgentUrl = "/enterprise/control/agent.php";

        private readonly LoginDetails loginDetails;
        private readonly HttpClient httpClient;
        private readonly string resultErrorCodeXmlName = string.Empty;
        private readonly string resultErrorTextXmlName = string.Empty;

        private string ApiUrl => (this.loginDetails?.Host ?? string.Empty) + AgentUrl;

        public PleskXMLApiService(LoginDetails loginDetails, bool ignoreCertificates = false)
        {
            this.loginDetails = loginDetails;

            this.httpClient = this.GetClient(ignoreCertificates);
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

        private HttpClient GetClient(bool ignoreCertificates = false)
        {
            if (ignoreCertificates)
            {
                // This shouldn't be used...please inform the user accordingly
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true;

                return new HttpClient(handler);
            }

            return new HttpClient();
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
            catch (AuthenticationException authException)
            {
                return this.HandleSSLAuthenticationException(authException);
            }
            catch (Exception ex)
            {
                if (ex.GetType().FullName == "Javax.Net.Ssl.SSLHandshakeException" ||
                    (ex.Message.Contains("The SSL connection could") && ex.InnerException?.GetType().FullName == "System.Security.Authentication.AuthenticationException") ||
                    ex.Message.Contains("Chain validation failed"))
                {
                    return this.HandleSSLAuthenticationException(ex);
                }

                return (new ErrorResult(ErrorCode.InternalError, ex), null);
            }
        }

        private (ErrorResult ErrorResult, Packet? Packet) HandleSSLAuthenticationException(Exception ex)
        {
            return (new ErrorResult(ErrorCode.SSLException, ex), null);
        }

        public void Dispose()
        {
            this.httpClient?.Dispose();
        }
    }
}

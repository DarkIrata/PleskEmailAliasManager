using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Data.PleskXmlApi;
using PleskEmailAliasManager.Enums;

namespace PleskEmailAliasManager.Services
{
    internal class PleskMailManager
    {
        private readonly PleskXMLApiService service;

        public PleskMailManager(PleskXMLApiService service)
        {
            this.service = service;
        }

        public async Task<ErrorResult> CheckAuthorization()
        {
            var packet = new Packet()
            {
                Customer = Customer.CreateMinimumGet(),
            };

            var result = await this.service.RequestAsync(packet).ConfigureAwait(false);

            return result.ErrorResult;
        }

        public async Task<(ErrorResult Result, List<DomainData> Domains)> GetDomainsAsync()
        {
            const string missingDomainName = "- Missing -";

            var packet = new Packet()
            {
                Site = Site.CreateMinimumGet(),
                Webspace = Webspace.CreateMinimumGet(),
            };

            packet.Site!.Get!.Dataset = new Dataset()
            {
                GenInfo = new GenInfo(),
            };

            packet.Webspace!.Get!.Dataset = new Dataset()
            {
                GenInfo = new GenInfo(),
            };

            var response = await this.service.RequestAsync(packet).ConfigureAwait(false);
            var domains = new List<DomainData>();

            if (response.ErrorResult.Successfully)
            {
                if (response.Packet!.Webspace != null)
                {
                    foreach (var webspace in response.Packet?.Webspace?.Get?.Result!)
                    {
                        domains.Add(new DomainData(webspace.Id, webspace.Data?.GenInfo?.Name ?? missingDomainName, DomainType.Webspace));
                    }
                }

                if (response.Packet!.Site != null)
                {

                    foreach (var site in response.Packet?.Site?.Get?.Result!)
                    {
                        domains.Add(new DomainData(site.Id, site.Data?.GenInfo?.Name ?? missingDomainName, DomainType.Site));
                    }
                }
            }

            return (response.ErrorResult, domains);
        }

        public async Task<(ErrorResult Result, List<MailData> Domains)> GetMailsAsync(DomainData domainData)
        {
            var packet = new Packet()
            {
                Mail = Mail.CreateMinimumGet(domainData.Id)
            };

            packet.Mail!.GetInfo!.Aliases = new Aliases();

            var response = await this.service.RequestAsync(packet).ConfigureAwait(false);
            var mails = new List<MailData>();

            if (response.ErrorResult.Successfully)
            {
                foreach (var mailAddress in response.Packet?.Mail?.GetInfo?.Result!)
                {
                    if (mailAddress.MailName == null)
                    {
                        continue;
                    }

                    mails.Add(new MailData(mailAddress.MailName.Id, mailAddress.MailName?.Name, mailAddress.MailName?.Aliases, domainData));
                }
            }

            return (response.ErrorResult, mails);
        }

        public async Task<(ErrorResult ErrorResult, Result? Result)> AddAlias(MailData mailData, string alias)
            => await this.AddAliases(mailData, new List<string>() { alias });

        public async Task<(ErrorResult ErrorResult, Result? Result)> AddAliases(MailData mailData, List<string> aliases)
        {
            var packet = new Packet()
            {
                Mail = new Mail()
                {
                    Update = new Update()
                    {
                        Add = new Add(mailData!.DomainData!.Id, mailData!.Name!)
                    }
                }
            };

            packet!.Mail!.Update!.Add!.Filter!.MailName!.Aliases = aliases;

            var response = await this.service.RequestAsync(packet).ConfigureAwait(false);
            var result = response.Packet?.Mail?.Update?.Add?.Result?.FirstOrDefault();

            return (response.ErrorResult, result);
        }

        public async Task<(ErrorResult ErrorResult, Result? Result)> DeleteAlias(MailData mailData, string alias)
            => await this.DeleteAliases(mailData, new List<string>() { alias });

        public async Task<(ErrorResult ErrorResult, Result? Result)> DeleteAliases(MailData mailData, List<string> aliases)
        {
            var packet = new Packet()
            {
                Mail = new Mail()
                {
                    Update = new Update()
                    {
                        Remove = new Remove(mailData!.DomainData!.Id, mailData!.Name!)
                    }
                }
            };

            packet!.Mail!.Update!.Remove!.Filter!.MailName!.Aliases = aliases;

            var response = await this.service.RequestAsync(packet).ConfigureAwait(false);
            var result = response.Packet?.Mail?.Update?.Remove?.Result?.FirstOrDefault();

            return (response.ErrorResult, result);
        }
    }
}

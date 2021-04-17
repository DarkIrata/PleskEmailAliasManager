using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using PleskEmailAliasManager.Models;
using PleskEmailAliasManager.Models.PleskXMLApi;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    public class MailsViewModel : PropertyChangedBase, IHandle<DomainData>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly PleskXMLApiService pleskXMLApiService;
        private readonly object mailsLock = new object();

        public ObservableCollection<MailData> Mails { get; }

        private MailData selectedMail;

        public MailData SelectedMail
        {
            get => this.selectedMail;

            set
            {
                this.eventAggregator.PublishOnBackgroundThread(value ?? new MailData(-1, string.Empty));
                this.Set(ref this.selectedMail, value, nameof(this.SelectedMail));
            }
        }

        public MailsViewModel(IEventAggregator eventAggregator, PleskXMLApiService pleskXMLApiService)
        {
            this.eventAggregator = eventAggregator ??
                    throw new ArgumentNullException(nameof(eventAggregator));

            this.pleskXMLApiService = pleskXMLApiService ??
                    throw new ArgumentNullException(nameof(pleskXMLApiService));

            this.eventAggregator.Subscribe(this);
            this.Mails = new ObservableCollection<MailData>();
            BindingOperations.EnableCollectionSynchronization(this.Mails, this.mailsLock);
        }

        public async Task LoadDataAsync(DomainData domainData)
        {
            var packet = new Packet()
            {
                Mail = Mail.CreateMinimumGet(domainData.Id)
            };

            packet.Mail.GetInfo.Aliases = new Aliases();

            var response = await this.pleskXMLApiService.RequestAsync(packet).ConfigureAwait(false);

            this.Mails.Clear();

            if (response.ErrorResult.ErrorCode == Data.ErrorCode.Success &&
                response.Packet.Mail != null &&
                response.Packet.Mail.GetInfo != null)
            {
                foreach (var mailAddress in response.Packet.Mail.GetInfo.Result)
                {
                    if (mailAddress.MailName == null)
                    {
                        continue;
                    }

                    this.Mails.Add(new MailData((int)mailAddress.MailName.Id, mailAddress.MailName.Name, mailAddress.MailName.Aliases, domainData));
                }
            }
        }

        public async void Handle(DomainData message)
        {
            await this.LoadDataAsync(message);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using PleskEmailAliasManager.Models;
using PleskEmailAliasManager.Models.PleskXMLApi;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    public class DomainsViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly PleskXMLApiService pleskXMLApiService;
        private readonly object domainsLock = new object();

        public ObservableCollection<DomainData> Domains { get; }

        private DomainData selectedDomain;

        public DomainData SelectedDomain
        {
            get => this.selectedDomain;

            set
            {
                this.eventAggregator.PublishOnBackgroundThread(value);
                this.Set(ref this.selectedDomain, value, nameof(this.SelectedDomain));
            }
        }

        public DomainsViewModel(IEventAggregator eventAggregator, PleskXMLApiService pleskXMLApiService)
        {
            this.eventAggregator = eventAggregator ??
                    throw new ArgumentNullException(nameof(eventAggregator));

            this.pleskXMLApiService = pleskXMLApiService ??
                    throw new ArgumentNullException(nameof(pleskXMLApiService));

            this.Domains = new ObservableCollection<DomainData>();
            BindingOperations.EnableCollectionSynchronization(this.Domains, this.domainsLock);

            this.LoadDataAsync().GetAwaiter();
        }

        public async Task LoadDataAsync()
        {
            const string unkownDomain = "UNKOWN";

            var packet = new Packet()
            {
                Site = Site.CreateMinimumGet(),
                Webspace = Webspace.CreateMinimumGet()
            };
            packet.Site.Get.Dataset = new Dataset()
            {
                GenInfo = new GenInfo()
            };

            packet.Webspace.Get.Dataset = packet.Site.Get.Dataset;

            var response = await this.pleskXMLApiService.RequestAsync(packet).ConfigureAwait(false);

            foreach (var webspace in response.Webspace.Get.Result)
            {
                this.Domains.Add(new DomainData(webspace.Id, webspace.Data?.GenInfo?.Name ?? unkownDomain));
            }

            foreach (var site in response.Site.Get.Result)
            {
                this.Domains.Add(new DomainData(site.Id, site.Data?.GenInfo?.Name ?? unkownDomain));
            }
        }
    }
}

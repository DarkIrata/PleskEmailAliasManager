using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IPUP.MVVM.Commands;
using IPUP.MVVM.Events;
using Microsoft.Win32;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Events;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    internal class MailsViewModel : PleskDataViewModel<MailData>
    {
        private DomainData? selectedDomain;

        public DomainData? SelectedDomain
        {
            get => this.selectedDomain;
            set => this.Set(ref this.selectedDomain, value, nameof(this.SelectedDomain));
        }

        private MailData? lastSelectedMailData = null;

        public DelegateCommandBase ExportMailsCommand { get; }

        public MailsViewModel(IEventAggregator eventAggregator, PleskMailManager provider)
            : base(eventAggregator, provider)
        {
            this.ExportMailsCommand = new DelegateCommand(() => this.ExecuteExportMails(), () => this.SelectedDomain != null);

            this.EventAggregator.Subscribe<SelectedDomainChangedEvent>(this.OnSelectedDomainChanged);
            this.EventAggregator.Subscribe<RefreshAndReselectMailEvent>(this.OnRefreshAndReselectMailRequested);
        }

        private void ExecuteExportMails()
        {
            var sfd = new SaveFileDialog()
            {
                FileName = $"{this.SelectedDomain!.Name}-Mails.json",
                Title = "Export Mail Alias List",
                Filter = "Json|*.json",
            };

            if (sfd.ShowDialog() == true)
            {

                var exportMailData = this.RawItems.Select(md => new ExportMailData(md.Name!, md.Aliases));
                File.WriteAllText(sfd.FileName,
                    JsonSerializer.Serialize(exportMailData, new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    })
                );
            }
        }

        private async void OnRefreshAndReselectMailRequested(RefreshAndReselectMailEvent args)
        {
            this.lastSelectedMailData = args.MailData;
            await this.ReloadItems();
        }

        internal override async Task OnItemsRefreshed()
        {
            await base.OnItemsRefreshed();

            if (this.lastSelectedMailData != null)
            {
                var item = this.Items.FirstOrDefault(md => md.Id == this.lastSelectedMailData.Id && md.Name == this.lastSelectedMailData.Name);
                if (item != null)
                {
                    this.SelectedItem = item;
                }
            }
        }

        private async void OnSelectedDomainChanged(SelectedDomainChangedEvent args)
        {
            this.SelectedDomain = args.DomainData;
            await this.ReloadItems();
            await this.RefreshControls();

            App.Current.Dispatcher.Invoke(() =>
            {
                this.ExportMailsCommand.RaiseCanExecuteChanged();
            });

            this.SelectedItem = null;
        }

        public override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            this.EventAggregator.Publish(new SelectedMailChangedEvent(this.SelectedItem));
        }

        internal override bool CanExecuteFlipSort() => true;

        internal override bool CanExecuteReload() => this.SelectedDomain != null;

        internal override async Task<(ErrorResult Result, List<MailData> Items)> RequestItems()
        {
            if (this.SelectedDomain == null)
            {
                return (ErrorResult.Success, new List<MailData>());
            }

            return await this.Provider.GetMailsAsync(this.SelectedDomain!);
        }
    }
}

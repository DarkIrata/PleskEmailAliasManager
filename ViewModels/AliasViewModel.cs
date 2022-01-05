using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IPUP.MVVM.Commands;
using IPUP.MVVM.Events;
using IPUP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Data.PleskXmlApi;
using PleskEmailAliasManager.Events;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    internal class AliasViewModel : EventViewModelBase, IComparable<AliasViewModel>
    {
        private readonly PleskMailManager pleskManager;
        private readonly MailData mailData;

        public string Alias { get; }

        private bool showDeleteConfirm = false;
        private bool canInteract = true;

        public bool ShowDeleteConfirm
        {
            get => this.showDeleteConfirm;
            set => this.Set(ref this.showDeleteConfirm, value, nameof(this.ShowDeleteConfirm));
        }

        public ICommand ShowDeleteConfirmCommand { get; }

        public DelegateCommandBase AbortDeletionCommand { get; }

        public DelegateCommandBase ConfirmDeletionCommand { get; }

        public AliasViewModel(IEventAggregator eventAggregator, PleskMailManager pleskManager, MailData mailData, string alias)
            : base(eventAggregator)
        {
            this.pleskManager = pleskManager;
            this.mailData = mailData;
            this.Alias = alias;

            this.ShowDeleteConfirmCommand = new DelegateCommand(() => this.ShowDeleteConfirm = true);
            this.AbortDeletionCommand = new DelegateCommand(() => this.ShowDeleteConfirm = false, () => this.canInteract);
            this.ConfirmDeletionCommand = new DelegateCommand(async () => await this.ExecuteDeleteCommand(), () => this.canInteract);
        }

        private async Task ExecuteDeleteCommand()
        {
            if (this.mailData == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.mailData!.Name))
            {
                await DialogHost.Show(new InfoDialogViewModel("Selected Mail has no Name set", PackIconKind.ErrorOutline), ShellViewModel.RootDialogIdentifier);
            }

            this.canInteract = false;

            var results = await this.pleskManager.DeleteAlias(this.mailData, this.Alias);
            if (results.ErrorResult.Successfully)
            {
                this.EventAggregator.Publish(new AliasDeletedEvent(this.Alias));
            }
            else
            {
                this.canInteract = true;
                await DialogHost.Show(new InfoDialogViewModel(results.ErrorResult.Message, PackIconKind.ErrorOutline), ShellViewModel.RootDialogIdentifier);
            }

            this.AbortDeletionCommand.RaiseCanExecuteChanged();
            this.ConfirmDeletionCommand.RaiseCanExecuteChanged();
        }

        public int CompareTo(AliasViewModel? other) => this.Alias.CompareTo(other?.Alias);
    }
}

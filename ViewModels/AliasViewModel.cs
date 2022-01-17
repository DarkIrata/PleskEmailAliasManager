using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IPUP.MVVM.Commands;
using IPUP.MVVM.Events;
using IPUP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;
using PleskEmailAliasManager.Data;
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

        public bool ShowDeleteConfirm
        {
            get => this.showDeleteConfirm;
            set => this.Set(ref this.showDeleteConfirm, value, nameof(this.ShowDeleteConfirm));
        }

        private bool showCopyChecked;

        public bool ShowCopyChecked
        {
            get => this.showCopyChecked;
            set => this.Set(ref this.showCopyChecked, value, nameof(this.ShowCopyChecked));
        }

        private bool canInteract = true;
        private CancellationTokenSource copyCheckCancellationTokenSource;

        public ICommand ShowDeleteConfirmCommand { get; }

        public ICommand CopyAliasAddressCommand { get; }

        public DelegateCommandBase AbortDeletionCommand { get; }

        public DelegateCommandBase ConfirmDeletionCommand { get; }

        public AliasViewModel(IEventAggregator eventAggregator, PleskMailManager pleskManager, MailData mailData, string alias)
            : base(eventAggregator)
        {
            this.pleskManager = pleskManager;
            this.mailData = mailData;
            this.Alias = alias;

            this.ShowDeleteConfirmCommand = new DelegateCommand(() => this.ShowDeleteConfirm = true);
            this.CopyAliasAddressCommand = new DelegateCommand(() => this.ExecuteCopyAliasAddress(), () => this.mailData?.DomainData != null);
            this.AbortDeletionCommand = new DelegateCommand(() => this.ShowDeleteConfirm = false, () => this.canInteract);
            this.ConfirmDeletionCommand = new DelegateCommand(async () => await this.ExecuteDeleteCommand(), () => this.canInteract);
        }

        private void ExecuteCopyAliasAddress()
        {
            try
            {
                if (!this.copyCheckCancellationTokenSource?.IsCancellationRequested ?? false)
                {
                    this.copyCheckCancellationTokenSource!.Cancel();
                }

                this.copyCheckCancellationTokenSource = new CancellationTokenSource();

                Clipboard.SetText($"{this.Alias}@{this.mailData?.DomainData?.Name}");
                this.ShowCopyChecked = true;

                var token = this.copyCheckCancellationTokenSource!.Token;
                Task.Run(async () =>
                {
                    await Task.Delay(1200);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    Application.Current.Dispatcher.Invoke(() => this.ShowCopyChecked = false);
                }, token);
            }
            catch
            {
                // Todo: Maybe add Logging?
            }
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

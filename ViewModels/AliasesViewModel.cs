using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IPUP.MVVM.Commands;
using IPUP.MVVM.Events;
using MaterialDesignThemes.Wpf;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Events;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    internal class AliasesViewModel : ListEventViewModelBase<AliasViewModel>
    {
        private readonly PleskMailManager pleskManager;

        private MailData? selectedMail;

        public MailData? SelectedMail
        {
            get => this.selectedMail;
            set => this.Set(ref this.selectedMail, value, nameof(this.SelectedMail));
        }

        public DelegateCommandBase AddAliasCommand { get; }

        public bool ListEmpty => this.Items.Count == 0 && this.SelectedMail != null;

        public AliasesViewModel(IEventAggregator eventAggregator, PleskMailManager pleskManager)
            : base(eventAggregator)
        {
            this.pleskManager = pleskManager;

            this.EventAggregator.Subscribe<SelectedMailChangedEvent>(this.OnSelectedMailChanged);
            this.EventAggregator.Subscribe<AliasDeletedEvent>(this.OnAliasDeleted);

            this.AddAliasCommand = new DelegateCommand(async () => await this.ExecuteAddAlias(), () => !string.IsNullOrEmpty(this.FilterText));
        }

        private async Task ExecuteAddAlias()
        {
            var alias = this.FilterText;
            if (string.IsNullOrEmpty(alias) || this.SelectedMail == null)
            {
                return;
            }

            var results = await this.pleskManager.AddAlias(this.SelectedMail!, alias!);
            if (results.ErrorResult.Successfully)
            {
                if (this.SelectedMail.Aliases == null)
                {
                    this.SelectedMail.Aliases = new List<string>();
                }

                this.SelectedMail.Aliases!.Add(alias);
                await this.ReloadItems();
                await this.RefreshControls();

                this.NotifyPropertyChanged(nameof(this.ListEmpty));
            }
            else
            {
                await DialogHost.Show(new InfoDialogViewModel(results.ErrorResult.Message, PackIconKind.ErrorOutline), ShellViewModel.RootDialogIdentifier);
            }
        }

        private void OnAliasDeleted(AliasDeletedEvent args)
        {
            var aliasItem = this.RawItems.Where(i => i.Alias == args.Alias).FirstOrDefault();
            if (aliasItem != null)
            {
                if (this.Items.Contains(aliasItem))
                {
                    this.Items.Remove(aliasItem);
                }

                this.RawItems.Remove(aliasItem);
            }

            this.NotifyPropertyChanged(nameof(this.ListEmpty));
        }

        private async void OnSelectedMailChanged(SelectedMailChangedEvent args)
        {
            this.SelectedMail = args.MailData;
            await this.ReloadItems();
            await this.RefreshControls();

            this.SelectedItem = null;
            this.FilterText = string.Empty;
        }

        internal override void OnFilterTextChanged()
        {
            base.OnFilterTextChanged();
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.AddAliasCommand.RaiseCanExecuteChanged();
            });

            this.NotifyPropertyChanged(nameof(this.ListEmpty));
        }

        internal override bool CanExecuteFlipSort() => true;

        internal override bool CanExecuteReload() => this.SelectedMail != null;

        internal override IEnumerable<AliasViewModel> GetSortedAndFilteredItems()
        {
            var filteredItems = this.RawItems.Where(i => i.Alias.Contains(this.FilterText!));
            if (!this.SortAscending)
            {
                return filteredItems.OrderByDescending(i => i);
            }

            return filteredItems.OrderBy(i => i);
        }

        internal override async Task ExecuteReload()
        {
            await this.EventAggregator.PublishAsync(new RefreshAndReselectMailEvent(this.SelectedMail!));
            await base.ExecuteReload();

            this.NotifyPropertyChanged(nameof(this.ListEmpty));
        }

        internal override Task<(ErrorResult Result, List<AliasViewModel> Items)> RequestItems()
            => Task.FromResult((ErrorResult.Success, this.SelectedMail?.Aliases?.Select(a => new AliasViewModel(
                this.EventAggregator,
                this.pleskManager,
                this.SelectedMail,
                a)).ToList() ?? new List<AliasViewModel>()));
    }
}

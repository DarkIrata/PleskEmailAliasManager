using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using PleskEmailAliasManager.Models;
using PleskEmailAliasManager.Models.PleskXMLApi;
using PleskEmailAliasManager.Services;
using PleskEmailAliasManager.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace PleskEmailAliasManager.ViewModels
{
    public class AliasesViewModel : PropertyChangedBase, 
        IHandle<MailData>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly PleskXMLApiService pleskXMLApiService;
        private readonly ISnackbarMessageQueue snackbarMessageQueue;
        private readonly object aliasLock = new object();

        public ObservableCollection<string> Aliases { get; }

        private string selectedAlias;

        public string SelectedAlias
        {
            get => this.selectedAlias;
            set => this.Set(ref this.selectedAlias, value, nameof(this.SelectedAlias));
        }

        private MailData activeMailData;

        public MailData ActiveMailData
        {
            get => this.activeMailData;
            set => this.Set(ref this.activeMailData, value, nameof(this.ActiveMailData));
        }

        private string newAliasName;

        public string NewAliasName
        {
            get => this.newAliasName;
            set => this.Set(ref this.newAliasName, value, nameof(this.NewAliasName));
        }

        public AliasesViewModel(IEventAggregator eventAggregator, PleskXMLApiService pleskXMLApiService, ISnackbarMessageQueue snackbarMessageQueue)
        {
            this.eventAggregator = eventAggregator ??
                    throw new ArgumentNullException(nameof(eventAggregator));

            this.pleskXMLApiService = pleskXMLApiService ??
                    throw new ArgumentNullException(nameof(pleskXMLApiService));

            this.snackbarMessageQueue = snackbarMessageQueue ??
                    throw new ArgumentNullException(nameof(snackbarMessageQueue));

            this.eventAggregator.Subscribe(this);
            this.Aliases = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(this.Aliases, this.aliasLock);
        }

        public void RequestDeleteAlias(string alias)
        {
            if (this.ActiveMailData.DomainData == null)
            {
                DialogHost.Show(CaliWPFUtilities.GetBindedUIElement(new InfoDialogViewModel(PackIconKind.WarningOutline, "Missing Domain data")), "ShellDialog", this.InfoDialogClosed);
                return;
            }

            this.SelectedAlias = alias;
            DialogHost.Show(CaliWPFUtilities.GetBindedUIElement(new YesNoDialogViewModel($"Delete the alias '{alias}'?")), "ShellDialog", this.DeleteYesNoDialogClosedAsync);
        }

        public bool CanRequestAddAlias() => this.ActiveMailData != null;

        public void RequestAddAlias()
        {
            if (string.IsNullOrEmpty(this.NewAliasName))
            {
                DialogHost.Show(CaliWPFUtilities.GetBindedUIElement(new InfoDialogViewModel(PackIconKind.WarningOutline, "Missing new Alias name")), "ShellDialog", this.InfoDialogClosed);
                return;
            }

            DialogHost.Show(CaliWPFUtilities.GetBindedUIElement(new YesNoDialogViewModel($"Create the new alias '{this.NewAliasName}@{this.ActiveMailData.DomainData.Name}'?")), "ShellDialog", this.AddYesNoDialogClosedAsync);
        }

        public async Task AddAlias(string alias)
        {
            var packet = new Packet()
            {
                Mail = new Mail()
                {
                    Update = new Update()
                    {
                        Add = new Add(this.ActiveMailData.DomainData.Id, this.ActiveMailData.Name)
                    }
                }
            };
            packet.Mail.Update.Add.Filter.MailName.Aliases = new List<string>() { alias };

            var result = await this.pleskXMLApiService.RequestAsync(packet).ConfigureAwait(false);
            var addResult = result?.Mail?.Update?.Add?.Result?.FirstOrDefault();

            var msg = "something is fishy..";
            var icon = PackIconKind.Fish;
            if (addResult == null)
            {
                msg = $"Error - Missing result from request";
                icon = PackIconKind.ErrorOutline;
            }
            else if (addResult.Status.ToLower() == "error")
            {
                msg = $"Error - {addResult.ErrorCode}{Environment.NewLine}{addResult.ErrorText}";
                icon = PackIconKind.ErrorOutline;
            }
            else if (addResult.Status.ToLower() != "ok")
            {
                msg = $"Error - Unkown status '{addResult.Status}'{Environment.NewLine}{addResult.ErrorText}";
                icon = PackIconKind.WarningOutline;
            }
            else if (addResult.Status.ToLower() == "ok")
            {
                msg = $"Success! The alias '{alias}@{this.ActiveMailData.DomainData.Name}' was added!";
                icon = PackIconKind.CheckCircleOutline;
                this.Aliases.Add(alias);
                this.NewAliasName = string.Empty;
            }

            var uiElement = System.Windows.Application.Current.Dispatcher.Invoke<UIElement>(() => CaliWPFUtilities.GetBindedUIElement(new InfoDialogViewModel(icon, msg)));
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => DialogHost.Show(uiElement, "ShellDialog", this.InfoDialogClosed));
        }

        public async Task DeleteAlias(string alias)
        {
            var packet = new Packet()
            {
                Mail = new Mail()
                {
                    Update = new Update()
                    {
                        Remove = new Remove(this.ActiveMailData.DomainData.Id, this.ActiveMailData.Name)
                    }
                }
            };
            packet.Mail.Update.Remove.Filter.MailName.Aliases = new List<string>() { alias };

            var result = await this.pleskXMLApiService.RequestAsync(packet).ConfigureAwait(false);
            var removeResult = result?.Mail?.Update?.Remove?.Result?.FirstOrDefault();

            var msg = "something is fishy..";
            var icon = PackIconKind.Fish;
            if (removeResult == null)
            {
                msg = $"Error - Missing result from request";
                icon = PackIconKind.ErrorOutline;
            }
            else if (removeResult.Status.ToLower() == "error")
            {
                msg = $"Error - {removeResult.ErrorCode}{Environment.NewLine}{removeResult.ErrorText}";
                icon = PackIconKind.ErrorOutline;
            }
            else if (removeResult.Status.ToLower() != "ok")
            {
                msg = $"Error - Unkown status '{removeResult.Status}'{Environment.NewLine}{removeResult.ErrorText}";
                icon = PackIconKind.WarningOutline;
            }
            else if (removeResult.Status.ToLower() == "ok")
            {
                msg = $"Success! The alias '{alias}' was removed!";
                icon = PackIconKind.CheckCircleOutline;
                this.Aliases.Remove(alias);
            }

            var uiElement = System.Windows.Application.Current.Dispatcher.Invoke<UIElement>(() => CaliWPFUtilities.GetBindedUIElement(new InfoDialogViewModel(icon, msg)));
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => DialogHost.Show(uiElement, "ShellDialog", this.InfoDialogClosed));
        }

        public void InfoDialogClosed(object sender, DialogClosingEventArgs eventArgs)
        {
        }

        public async void AddYesNoDialogClosedAsync(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool create && create)
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => this.AddAlias(this.NewAliasName));
            }
        }

        public async void DeleteYesNoDialogClosedAsync(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool delete && delete)
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => this.DeleteAlias(this.SelectedAlias));
            }
        }

        public void Handle(MailData message)
        {
            this.Aliases.Clear();
            if (message.Id == -1 && string.IsNullOrEmpty(message.Name) && string.IsNullOrEmpty(message.DomainData?.Name))
            {
                this.ActiveMailData = null;
                return;
            }

            this.ActiveMailData = message;
            foreach (var alias in message.Aliases)
            {
                this.Aliases.Add(alias);
            }

            this.snackbarMessageQueue.Enqueue($"Got Message {message.DomainData.Name}");
        }
    }
}

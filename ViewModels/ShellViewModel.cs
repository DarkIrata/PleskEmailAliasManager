using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using PleskEmailAliasManager.Models;
using PleskEmailAliasManager.Services;
using PleskEmailAliasManager.Utilities;

namespace PleskEmailAliasManager.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private const string LoginDataFile = "login.xml";

        private readonly IEventAggregator eventAggregator;
        private readonly LoginDetails loginDetails = new LoginDetails();
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(LoginDetails));
        private PleskXMLApiService apiService;

        public string Title => "PLESK E-Mail Alias Manager";

        private bool isDialogVisible = true;

        public bool IsDialogVisible
        {
            get => this.isDialogVisible;
            set => this.Set(ref this.isDialogVisible, value, nameof(this.IsDialogVisible));
        }

        private DomainsViewModel domains;

        public DomainsViewModel Domains
        {
            get => this.domains;
            set => this.Set(ref this.domains, value, nameof(this.Domains));
        }

        private MailsViewModel mails;

        public MailsViewModel Mails
        {
            get => this.mails;
            set => this.Set(ref this.mails, value, nameof(this.Mails));
        }

        private AliasesViewModel aliases;

        public AliasesViewModel Aliases
        {
            get => this.aliases;
            set => this.Set(ref this.aliases, value, nameof(this.Aliases));
        }

        private ISnackbarMessageQueue snackbarMessageQueue;

        public ISnackbarMessageQueue SnackbarMessageQueue
        {
            get => this.snackbarMessageQueue;
            private set => this.Set(ref this.snackbarMessageQueue, value, nameof(this.SnackbarMessageQueue));
        }

        public ShellViewModel(IEventAggregator eventAggregator, ISnackbarMessageQueue snackbarMessageQueue)
        {
            this.eventAggregator = eventAggregator ??
                    throw new ArgumentNullException(nameof(eventAggregator));

            this.SnackbarMessageQueue = snackbarMessageQueue ??
                    throw new ArgumentNullException(nameof(snackbarMessageQueue));

            if (File.Exists(LoginDataFile))
            {
                using (var fw = File.OpenRead(LoginDataFile))
                {
                    this.loginDetails = (LoginDetails)this.serializer.Deserialize(fw);
                }
            }
        }

        public void OnDisplayed()
        {
            this.IsDialogVisible = false;
            DialogHost.Show(CaliWPFUtilities.GetBindedUIElement(new LoginViewModel(this.loginDetails)), "ShellDialog", this.DialogClosed);
        }

        public void OnClose(object eventArgs)
        {
            if (this.loginDetails?.SaveLoginDetails ?? false)
            {
                using (var fw = File.OpenWrite(LoginDataFile))
                {
                    this.serializer.Serialize(fw, this.loginDetails);
                }
            }
            else
            {
                if (File.Exists(LoginDataFile))
                {
                    File.Delete(LoginDataFile);
                }
            }
        }

        private void DialogClosed(object sender, DialogClosingEventArgs eventArgs)
        {
            this.apiService = new PleskXMLApiService(this.loginDetails);

            this.Domains = new DomainsViewModel(this.eventAggregator, this.apiService);
            this.Mails = new MailsViewModel(this.eventAggregator, this.apiService);
            this.Aliases = new AliasesViewModel(this.eventAggregator, this.apiService, this.snackbarMessageQueue);
        }
    }
}

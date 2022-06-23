using System.Reflection;
using IPUP.MVVM.Events;
using IPUP.MVVM.ViewModels;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    public class ShellViewModel : EventViewModelBase
    {
        public static string RootDialogIdentifier => "RootDialogHost";

        private bool loggedIn;

        public bool IsLoggedIn
        {
            get => this.loggedIn;
            set => this.Set(ref this.loggedIn, value, nameof(this.IsLoggedIn));
        }

        private ViewModelBase? activeViewModel;

        public ViewModelBase? ActiveViewModel
        {
            get => this.activeViewModel;
            set => this.Set(ref this.activeViewModel, value, nameof(this.ActiveViewModel));
        }
        public string Title => "PLESK E-Mail Alias Manager - " + Assembly.GetExecutingAssembly().GetName().Version;

        public ShellViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            this.SetupLoginViewModel();
        }

        private void OnLogin(PleskMailManager provider)
        {
            if (this.ActiveViewModel is LoginViewModel lvm)
            {
                lvm.OnLogin -= this.OnLogin;
                this.SetupAddressViewModel(provider);
            }
            else if (this.ActiveViewModel is AddressViewModel avm)
            {
                this.SetupLoginViewModel();
            }
        }

        private void SetupLoginViewModel()
        {
            var vm = new LoginViewModel();
            vm.OnLogin += this.OnLogin;
            this.ActiveViewModel = vm;
        }

        private void SetupAddressViewModel(PleskMailManager provider)
        {
            var vm = new AddressViewModel(this.EventAggregator, provider);
            this.ActiveViewModel = vm;
        }
    }
}

using System.Windows.Input;
using IPUP.MVVM.Commands;
using IPUP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;

namespace PleskEmailAliasManager.ViewModels
{
    internal class ConfirmDialogViewModel : ViewModelBase
    {
        private PackIconKind icon;

        public PackIconKind Icon
        {
            get => this.icon;
            set => this.Set(ref this.icon, value, nameof(this.Icon));
        }

        private string? title;

        public string? Title
        {
            get => this.title;
            set => this.Set(ref this.title, value, nameof(this.Title));
        }

        private string? text;

        public string? Text
        {
            get => this.text;
            set => this.Set(ref this.text, value, nameof(this.Text));
        }

        private string? confirmText;

        public string? ConfirmText
        {
            get => this.confirmText;
            set => this.Set(ref this.confirmText, value, nameof(this.ConfirmText));
        }

        private string? cancelText;

        public string? CancelText
        {
            get => this.cancelText;
            set => this.Set(ref this.cancelText, value, nameof(this.CancelText));
        }

        public ICommand ConfirmCommand { get; }

        public ICommand CancelCommand { get; }

        public ConfirmDialogViewModel(string? title, string? text, string? confirmText = "Confirm", string? cancelText = "Cancel", PackIconKind icon = PackIconKind.InfoCircleOutline)
        {
            this.Icon = icon;
            this.Title = title;
            this.Text = text;
            this.ConfirmText = confirmText;
            this.CancelText = cancelText;

            this.ConfirmCommand = new DelegateCommand(() => DialogHost.CloseDialogCommand.Execute(true, null));
            this.CancelCommand = new DelegateCommand(() => DialogHost.CloseDialogCommand.Execute(false, null));
        }
    }
}

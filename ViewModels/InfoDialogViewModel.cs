using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IPUP.MVVM.Commands;
using IPUP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;

namespace PleskEmailAliasManager.ViewModels
{
    internal class InfoDialogViewModel : ViewModelBase
    {
        private PackIconKind icon;

        public PackIconKind Icon
        {
            get => this.icon;
            set => this.Set(ref this.icon, value, nameof(this.Icon));
        }

        private string? text;

        public string? Text
        {
            get => this.text;
            set => this.Set(ref this.text, value, nameof(this.Text));
        }

        public ICommand OkCommand { get; }

        public InfoDialogViewModel(string? text, PackIconKind icon = PackIconKind.InfoCircleOutline)
        {
            this.Icon = icon;
            this.Text = text;

            this.OkCommand = new DelegateCommand(() => DialogHost.CloseDialogCommand.Execute(null, null));
        }
    }
}

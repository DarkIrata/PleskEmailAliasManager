using System;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;

namespace PleskEmailAliasManager.ViewModels
{
    public class InfoDialogViewModel : PropertyChangedBase
    {
        private PackIconKind icon;

        public PackIconKind Icon
        {
            get => this.icon;
            set => this.Set(ref this.icon, value, nameof(this.Icon));
        }

        private string text;

        public string Text
        {
            get => this.text;
            set => this.Set(ref this.text, value, nameof(this.Text));
        }

        public InfoDialogViewModel(PackIconKind icon, string text)
        {
            this.Icon = icon;
            this.Text = text ??
                    throw new ArgumentNullException(nameof(text));
        }

        public void Ok()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}

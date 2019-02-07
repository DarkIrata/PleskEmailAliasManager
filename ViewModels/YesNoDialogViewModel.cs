using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;

namespace PleskEmailAliasManager.ViewModels
{
    public class YesNoDialogViewModel : PropertyChangedBase
    {
        private string text;

        public string Text
        {
            get => this.text;
            set => this.Set(ref this.text, value, nameof(this.Text));
        }

        public YesNoDialogViewModel(string text)
        {
            this.Text = text ??
                    throw new ArgumentNullException(nameof(text));
        }

        public void Yes()
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
        }

        public void No()
        {
            DialogHost.CloseDialogCommand.Execute(false, null);
        }
    }
}

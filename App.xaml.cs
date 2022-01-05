using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using IPUP.MVVM.Events;
using PleskEmailAliasManager.ViewModels;
using PleskEmailAliasManager.Views;

namespace PleskEmailAliasManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IEventAggregator eventAggregator;
        private ShellViewModel shellViewModel;

        public App()
        {
            this.eventAggregator = new EventAggregator();
            this.shellViewModel = new ShellViewModel(this.eventAggregator);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.MainWindow = new ShellView() { DataContext = this.shellViewModel };
            this.MainWindow.Show();
        }
    }
}

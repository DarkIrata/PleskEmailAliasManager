using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Ninject;
using PleskEmailAliasManager.Utilities.Conventions;
using PleskEmailAliasManager.ViewModels;

namespace PleskEmailAliasManager
{
    public class AppBootstrapper : BootstrapperBase
    {
        private IKernel kernel;

        public AppBootstrapper()
        {
            this.Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(PasswordBoxBindHelper.BoundPasswordProperty, nameof(PasswordBox.Password), nameof(PasswordBox.PasswordChanged));
        }

        protected override void Configure()
        {
            this.kernel = new StandardKernel();

            this.kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            this.kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            this.kernel.Bind<ISnackbarMessageQueue>().To<SnackbarMessageQueue>().InSingletonScope();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            this.kernel.Dispose();
            base.OnExit(sender, e);
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            return this.kernel.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            this.kernel.Inject(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>();
        }
    }
}
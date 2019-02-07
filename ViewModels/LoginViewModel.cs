using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using PleskEmailAliasManager.Models;
using PleskEmailAliasManager.Models.PleskXMLApi;

namespace PleskEmailAliasManager.ViewModels
{
    public class LoginViewModel : PropertyChangedBase, IDataErrorInfo
    {
        private readonly LoginDetails loginDetails;

        public string Server
        {
            get => this.loginDetails.Server;
            set
            {
                if (this.loginDetails.Server != value)
                {
                    this.loginDetails.Server = value;
                    this.NotifyOfPropertyChange(nameof(this.Server) ?? string.Empty);
                    this.NotifyOfPropertyChange(nameof(this.CanLogin));
                }
            }
        }

        public string Username
        {
            get => this.loginDetails.Username;
            set
            {
                if (this.loginDetails.Username != value)
                {
                    this.loginDetails.Username = value;
                    this.NotifyOfPropertyChange(nameof(this.Username) ?? string.Empty);
                    this.NotifyOfPropertyChange(nameof(this.CanLogin));
                }
            }
        }

        public string Password
        {
            get => this.loginDetails.Password;
            set
            {
                if (this.loginDetails.Password != value)
                {
                    this.loginDetails.Password = value;
                    this.NotifyOfPropertyChange(nameof(this.Password) ?? string.Empty);
                    this.NotifyOfPropertyChange(nameof(this.CanLogin));
                }
            }
        }

        public bool SaveLoginDetails
        {
            get => this.loginDetails.SaveLoginDetails;
            set
            {
                if (this.loginDetails.SaveLoginDetails != value)
                {
                    this.loginDetails.SaveLoginDetails = value;
                    this.NotifyOfPropertyChange(nameof(this.SaveLoginDetails));
                }
            }
        }

        public bool CanLogin => !string.IsNullOrEmpty(this.Server) && !string.IsNullOrEmpty(this.Username);

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == nameof(this.Server))
                {
                    if (string.IsNullOrEmpty(this.Server))
                    {
                        result = "Server can't be empty!";
                    }
                }
                else if (columnName == nameof(this.Username))
                {
                    if (string.IsNullOrEmpty(this.Username))
                    {
                        result = "Username can't be empty!";
                    }
                }

                return result;
            }
        }

        public void Login()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public LoginViewModel(LoginDetails loginDetails)
        {
            this.loginDetails = loginDetails ??
                    throw new ArgumentNullException(nameof(loginDetails));
        }
    }
}

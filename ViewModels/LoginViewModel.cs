using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IPUP.MVVM.Commands;
using IPUP.MVVM.ViewModels;
using PleskEmailAliasManager.Data;
using PleskEmailAliasManager.Services;

namespace PleskEmailAliasManager.ViewModels
{
    internal class LoginViewModel : ViewModelBase, IDataErrorInfo
    {
        private const string LoginDataFileName = "settings.json";

        private readonly LoginDetails loginDetails;

        public event Action<PleskMailManager>? OnLogin;

        public string? Host
        {
            get => this.loginDetails.Host;
            set
            {
                this.loginDetails.Host = value;
                this.NotifyPropertyChanged(nameof(this.Host));
                this.LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string? Username
        {
            get => this.loginDetails.Username;
            set
            {
                this.loginDetails.Username = value;
                this.NotifyPropertyChanged(nameof(this.Username));
                this.LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string? Password
        {
            get => this.loginDetails.Password;
            set
            {
                this.loginDetails.Password = value;
                this.NotifyPropertyChanged(nameof(this.Password));
                this.LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private bool saveLogin = true;

        public bool SaveLogin
        {
            get => this.saveLogin;
            set => this.Set(ref this.saveLogin, value, nameof(this.SaveLogin));
        }

        private bool isWorking;

        public bool IsWorking
        {
            get => this.isWorking;
            set
            {
                this.Set(ref this.isWorking, value, nameof(this.IsWorking));
                this.LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string? errorText;

        public string? ErrorText
        {
            get => this.errorText;
            set => this.Set(ref this.errorText, value, nameof(this.ErrorText));
        }

        public DelegateCommand LoginCommand { get; }

#pragma warning disable CS8603 // Possible null reference return.
        public string Error => null;
#pragma warning restore CS8603 // Possible null reference return.

        public string this[string columnName]
        {
            get
            {
                string? result = null;
                if (columnName == nameof(this.Host))
                {
                    if (string.IsNullOrEmpty(this.Host))
                    {
                        result = "Server can't be empty! Example: http://HOST.TLD:8443";
                    }
                }
                else if (columnName == nameof(this.Username))
                {
                    if (string.IsNullOrEmpty(this.Username))
                    {
                        result = "Username can't be empty!";
                    }
                }
                else if (columnName == nameof(this.Password))
                {
                    if (string.IsNullOrEmpty(this.Password))
                    {
                        result = "Password can't be empty!";
                    }
                }

#pragma warning disable CS8603 // Possible null reference return.
                return result;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        public LoginViewModel()
        {
            this.loginDetails = new LoginDetails();

            if (File.Exists(LoginDataFileName))
            {
                try
                {
                    this.loginDetails = JsonSerializer.Deserialize<LoginDetails>(File.ReadAllText(LoginDataFileName))!;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            this.LoginCommand = new DelegateCommand(() => this.ExecuteLogin(), () => this.CanExecuteLogin());
        }

        public bool CanExecuteLogin() => !string.IsNullOrEmpty(this.Host) && 
            !string.IsNullOrEmpty(this.Username) && 
            !string.IsNullOrEmpty(this.Password) &&
            !this.IsWorking;

        private async void ExecuteLogin()
        {
            var provider = new PleskMailManager(new PleskXMLApiService(this.loginDetails));
            this.IsWorking = true;

            var result = await provider.CheckAuthorization();

            this.ErrorText = String.Empty;
            if (!result.Successfully)
            {
                this.ErrorText = result.Message ?? string.Empty;
            }

            this.IsWorking = false;
            try
            {
                this.HandleSaveLoginData();
                this.OnLogin?.Invoke(provider);
            }
            catch (Exception ex)
            {
                this.ErrorText = "Something went wrong: " + ex.Message;
            }
        }

        private void HandleSaveLoginData()
        {
            if (File.Exists(LoginDataFileName))
            {
                File.Delete(LoginDataFileName);
            }

            if (this.SaveLogin)
            {
                File.WriteAllText(LoginDataFileName, 
                    JsonSerializer.Serialize(this.loginDetails, new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    })
                );
            }
        }
    }
}

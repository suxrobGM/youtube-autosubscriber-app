using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriber.ViewModels
{
    public class AddAccountViewModel : BindableBase
    {
        private string _emailField;
        private string _passwordField;

        public string EmailField { get => _emailField; set { SetProperty(ref _emailField, value); } }
        public string PasswordField { get => _passwordField; set { SetProperty(ref _passwordField, value); } }
        public DelegateCommand AddAccountCommand { get; }

        public AddAccountViewModel()
        {
            AddAccountCommand = new DelegateCommand(() =>
            {

            });
        }
    }
}

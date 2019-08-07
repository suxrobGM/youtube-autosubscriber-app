using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using YouTubeSubscriber.Data;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.ViewModels
{
    public class AddAccountViewModel : BindableBase
    {
        private string _emailField;
        private string _passwordField;
        private string _statusText;
        private readonly ApplicationDbContext _context;

        public string EmailField { get => _emailField; set { SetProperty(ref _emailField, value); AddAccountCommand.RaiseCanExecuteChanged(); } }
        public string PasswordField { get => _passwordField; set { SetProperty(ref _passwordField, value); AddAccountCommand.RaiseCanExecuteChanged(); } }
        public string StatusText { get => _statusText; set { SetProperty(ref _statusText, value); } }
        public DelegateCommand AddAccountCommand { get; }
        public ObservableCollection<Account> Accounts { get; set; }

        public AddAccountViewModel(ApplicationDbContext context)
        {
            _context = context;
            StatusText = "";

            AddAccountCommand = new DelegateCommand(() =>
            {
                var account = new Account()
                {
                    Email = EmailField,
                    Password = PasswordField
                };
                _context.Accounts.Add(account);

                try
                {
                    _context.SaveChanges();
                    Accounts.Add(account);
                }
                catch (DbUpdateException)
                {
                    StatusText += $"ERROR: Account already exists in database\n";
                    return;
                }
                
                StatusText += $"Added account to database \n{account}";
                
            }, CanExecuteAddCommand);
        }

        private bool CanExecuteAddCommand() => !string.IsNullOrWhiteSpace(EmailField) && !string.IsNullOrWhiteSpace(PasswordField);
    }
}

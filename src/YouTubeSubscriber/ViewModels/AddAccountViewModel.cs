using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using YouTubeSubscriber.Data;
using YouTubeSubscriber.Models;
using YouTubeSubscriber.Services;

namespace YouTubeSubscriber.ViewModels
{
    public class AddAccountViewModel : BindableBase
    {                
        private readonly ApplicationDbContext _context;
        private Account _account;
        private string _statusText;

        public string StatusText { get => _statusText; set { SetProperty(ref _statusText, value); } }
        public Account Account { get => _account; set { SetProperty(ref _account, value); } }
        public DelegateCommand AddAccountCommand { get; }
        public ObservableCollection<Account> Accounts { get; set; }

        public AddAccountViewModel(ApplicationDbContext context)
        {
            _context = context;
            StatusText = "";
            Account = new Account();

            AddAccountCommand = new DelegateCommand(() =>
            {
                var account = new Account()
                {
                    Email = Account.Email,
                    Password = Account.Password
                };

                if (string.IsNullOrWhiteSpace(Account.Email))
                {
                    MessageBox.Show("Please fill email field");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Account.Password))
                {
                    MessageBox.Show("Please fill password field");
                    return;
                }

                var accountFromDb = _context.Accounts.Where(i => i.Email == Account.Email).FirstOrDefault();

                if (accountFromDb != null)
                {
                    StatusText += $"ERROR: Account already exists in database\n";
                    return;
                }
                else
                {
                    using (var accountService = new GoogleAccountService(account))
                    {
                        accountService.OnProcessing += AccountService_OnProcessing;

                        if (accountService.VerifyAccount())
                        {
                            account.IsVerified = true;

                            _context.Accounts.Add(account);
                            _context.SaveChanges();
                            Accounts.Add(account);
                            StatusText += $"Added account to database \n{account}";
                            Account.Email = "";
                            Account.Password = "";
                        }
                    }
                    
                }
            });
        }

        private void AccountService_OnProcessing(object sender, System.EventArgs e)
        {
            var message = sender as string;
            StatusText += message + "\n";
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using YouTubeSubscriber.Data;
using YouTubeSubscriber.Helpers;
using YouTubeSubscriber.Models;
using YouTubeSubscriber.Views;

namespace YouTubeSubscriber.ViewModels
{
    public class AccountsViewModel : BindableBase
    {
        private readonly ApplicationDbContext _context;
        private Account _selectedAccount;
       
        public ObservableCollection<Account> Accounts { get; }      
        public DelegateCommand ImportFromExcelCommand { get; }
        public DelegateCommand AddAccountCommand { get; }
        public DelegateCommand RemoveCommand { get; }

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                SetProperty(ref _selectedAccount, value);
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }

        public AccountsViewModel(ApplicationDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
            Accounts = new ObservableCollection<Account>(_context.Accounts);
            //GenerateAccounts();

            AddAccountCommand = new DelegateCommand(() =>
            {
                var addAccountView = new AddAccountView();
                (addAccountView.DataContext as AddAccountViewModel).Accounts = Accounts;
                addAccountView.ShowDialog();
            });

            RemoveCommand = new DelegateCommand(() =>
            {
                var msgResult = MessageBox.Show($"Do you want to remove account from database?\n{SelectedAccount}", "Remove account", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (msgResult == DialogResult.Yes)
                {                  
                    _context.Accounts.Remove(SelectedAccount);
                    _context.SaveChanges();
                    Accounts.Remove(SelectedAccount);
                }

            }, CanExecuteRemoveCommand);

            ImportFromExcelCommand = new DelegateCommand(() =>
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Multiselect = false;
                    dialog.Filter = "Excel file (.xlsx)|*.xlsx";
                    dialog.ShowDialog();

                    if (!string.IsNullOrWhiteSpace(dialog.FileName))
                    {
                        var accounts = ExcelParser.ParseAccounts(dialog.FileName);
                        var msgResult = MessageBox.Show($"Do you want to import {accounts.Count} accounts from {Path.GetFileName(dialog.FileName)} ?", "Excel Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (msgResult == DialogResult.Yes)
                        {                          
                            _context.Accounts.AddRange(accounts);
                            _context.SaveChanges();
                            Accounts.AddRange(accounts);
                        }
                    }                                      
                }
            });
        }

        private void GenerateAccounts()
        {
            var rnd = new Random();
            var channels = new Channel[]
            {
                new Channel()
                {
                    Title = "UzbekFilmsHD",
                    Url = "https://www.youtube.com/user/UzbekFilmsHD",
                },
                new Channel()
                {
                    Title = "Dumbazz",
                    Url = "https://www.youtube.com/channel/UC64Dw03B-EgwrNERMOy1u5w",
                },
                new Channel()
                {
                    Title = "NevoFilms",
                    Url = "https://www.youtube.com/channel/UC8O6rvkAJqGVWMlT9zgdH9Q",
                },
                new Channel()
                {
                    Title = "KinoCheck International",
                    Url = "https://www.youtube.com/user/Filme",
                },
                new Channel()
                {
                    Title = "Fast & Furious",
                    Url = "https://www.youtube.com/user/fastandfuriousmovie",
                },
            };            

            for (int i = 0; i < 1000; i++)
            {
                var account = new Account()
                {
                    Email = $"email{i}@gmail.com",
                    Password = rnd.Next(100000, 999999).ToString(),
                };
                var randomChannel = channels[rnd.Next(0, 4)];
                account.SubscribedChannels.Add(new ChannelAccount()
                {
                    AccountId = account.Id,
                    Account = account,
                    Channel = randomChannel,
                    ChannelId = randomChannel.Id
                });
                Accounts.Add(account);
            }
        }
        private bool CanExecuteRemoveCommand() => SelectedAccount != null;
    }
}

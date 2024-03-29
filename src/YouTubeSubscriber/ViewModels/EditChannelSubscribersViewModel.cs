﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using YouTubeSubscriber.Data;
using YouTubeSubscriber.Models;
using YouTubeSubscriber.Services;

namespace YouTubeSubscriber.ViewModels
{
    public class EditChannelSubscribersViewModel : BindableBase
    {
        private bool _isBusy;
        private bool _valueChanged;
        private int _totalAccounts;
        private int _subsAccountsCount;
        private int _initSubsAccountsValue;
        private string _statusText;
        private Channel _channel;
        private readonly ApplicationDbContext _context;

        public bool IsBusy { get => _isBusy; set { SetProperty(ref _isBusy, value); } }
        public int TotalAccounts { get => _totalAccounts; set { SetProperty(ref _totalAccounts, value); } }
        public int InitSubsAccountsCount { get => _initSubsAccountsValue; set { SetProperty(ref _initSubsAccountsValue, value); } }
        public string StatusText { get => _statusText; set { SetProperty(ref _statusText, value); } }
        public Channel Channel { get => _channel; set { SetProperty(ref _channel, value); } }
        public int SubsAccountsCount
        {
            get => _subsAccountsCount;
            set
            {
                SetProperty(ref _subsAccountsCount, value);
                _valueChanged = true;
                StartProcessCommand.RaiseCanExecuteChanged();
            }
        }


        public DelegateCommand StartProcessCommand { get; }
        public DelegateCommand StopProcessCommand { get; }
        public DelegateCommand UpdateSubscriberCountCommand { get; }
        public DelegateCommand WindowLoadedCommand { get; }

        public EditChannelSubscribersViewModel(ApplicationDbContext context)
        {
            _context = context;            
            StatusText = "";        

            StartProcessCommand = new DelegateCommand(() =>
            {             
                Task.Run(() =>
                {
                    IsBusy = true;
                    int accountsToProcessCount = SubsAccountsCount - InitSubsAccountsCount;

                    using (var subscriberService = new ChannelSubscriberService(_context, Channel))
                    {
                        if (accountsToProcessCount > InitSubsAccountsCount) // Start subscribing process
                        {
                            subscriberService.OnProcessing += SubscriberService_OnProcessing;
                            subscriberService.SubscribeToChannel(accountsToProcessCount);
                        }
                        else if (accountsToProcessCount < InitSubsAccountsCount) // Start unsubscribing process
                        {
                            //subscriberService.OnProcessing += Automatization_OnProcessing;
                            //subscriberService.UnsubscribeToChannel(accountsToProcessCount);
                        }

                        UpdateSubscriberCountCommand.Execute();
                    }
                });

            }, CanExecuteStartSubscription);

            StopProcessCommand = new DelegateCommand(() =>
            {
                IsBusy = false;

            }, CanExecuteStopSubscription);            

            UpdateSubscriberCountCommand = new DelegateCommand(() =>
            {
                Task.Run(() =>
                {
                    StatusText += "Updating channel subscriber count...\n";
                    IsBusy = true;
                    var oldValue = Channel.SubscriberCount;
                    var channel = _context.Channels.Where(i => i.Id == Channel.Id).First();

                    using (var subscriberService = new ChannelSubscriberService(_context, channel, true))
                    {                                            
                        var subscriberCount = subscriberService.GetSubscribersCount();
                        channel.SubscriberCount = subscriberCount;
                        Channel.SubscriberCount = subscriberCount;
                        _context.SaveChanges();                        
                    }

                    StatusText += $"Updated subscriber count from {oldValue} to {Channel.SubscriberCount}\n";
                    IsBusy = false;
                });
                
            });

            WindowLoadedCommand = new DelegateCommand(() =>
            {
                var subscribedAccounts = _context.Accounts.Where(i => i.SubscribedChannels.Where(x => x.ChannelId == _channel.Id).Any()).Count();
                TotalAccounts = _context.Accounts.Count();
                SubsAccountsCount = subscribedAccounts;
                InitSubsAccountsCount = subscribedAccounts;
            });
        }

        private void SubscriberService_OnProcessing(object sender, System.EventArgs e)
        {
            var message = sender as string;
            StatusText += message + "\n";
        }

        private bool CanExecuteStartSubscription() => !IsBusy && _valueChanged;
        private bool CanExecuteStopSubscription() => IsBusy;
    }
}

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
        private int _subsAccounts;
        private int _initSubsAccountsValue;
        private string _statusText;
        private Channel _channel;
        private readonly ApplicationDbContext _context;

        public bool IsBusy { get => _isBusy; set { SetProperty(ref _isBusy, value); } }
        public int TotalAccounts { get => _totalAccounts; set { SetProperty(ref _totalAccounts, value); } }
        public string StatusText { get => _statusText; set { SetProperty(ref _statusText, value); } }
        public Channel Channel { get => _channel; set { SetProperty(ref _channel, value); } }
        public int SubsAccounts
        {
            get => _subsAccounts;
            set
            {
                SetProperty(ref _subsAccounts, value);
                _valueChanged = true;
                StartSubscriptionCommand.RaiseCanExecuteChanged();
            }
        }


        public DelegateCommand StartSubscriptionCommand { get; }
        public DelegateCommand StopSubscriptionCommand { get; }
        public DelegateCommand UpdateSubscriberCountCommand { get; }
        public DelegateCommand WindowLoadedCommand { get; }

        public EditChannelSubscribersViewModel(ApplicationDbContext context)
        {
            _context = context;            
            StatusText = "";        

            StartSubscriptionCommand = new DelegateCommand(() =>
            {             
                Task.Run(() =>
                {
                    IsBusy = true;

                    using (var automatization = new Automatization())
                    {                        
                        automatization.OnSubscribing += Automatization_OnSubscribing;
                        //automatization.SubscribeToChannel(Channel, );
                    }
                });

            }, CanExecuteStartSubscription);

            StopSubscriptionCommand = new DelegateCommand(() =>
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

                    using (var automatization = new Automatization(true))
                    {                        
                        var channel = _context.Channels.Where(i => i.Id == Channel.Id).First();
                        var subscriberCount = automatization.GetSubscribersCount(channel);
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
                SubsAccounts = _context.Channels.Where(i => i.Id == Channel.Id).First().SubscribedAccounts.Count;
                TotalAccounts = _context.Accounts.Count();
                _initSubsAccountsValue = SubsAccounts;
            });
        }

        private void Automatization_OnSubscribing(object sender, System.EventArgs e)
        {
            var message = sender as string;
            StatusText += message + "\n";
        }

        private bool CanExecuteStartSubscription() => !IsBusy && _valueChanged;
        private bool CanExecuteStopSubscription() => IsBusy;
    }
}

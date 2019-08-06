using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
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
        private string _statusText;
        private Channel _channel;

        public bool IsBusy { get => _isBusy; set { SetProperty(ref _isBusy, value); } }
        public int TotalAccounts { get => _totalAccounts; set { SetProperty(ref _totalAccounts, value); } }
        public int SubsAccounts { get => _subsAccounts; set { SetProperty(ref _subsAccounts, value); } }
        public string StatusText { get => _statusText; set { SetProperty(ref _statusText, value); } }
        public Channel Channel { get => _channel; set { SetProperty(ref _channel, value); } }
        

        public DelegateCommand StartSubscriptionCommand { get; }
        public DelegateCommand StopSubscriptionCommand { get; }
        public DelegateCommand SubsAccountsChangedCommand { get; }

        public EditChannelSubscribersViewModel()
        {
            StatusText = "";            

            StartSubscriptionCommand = new DelegateCommand(() =>
            {
                IsBusy = true;
                Task.Run(() =>
                {
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

            SubsAccountsChangedCommand = new DelegateCommand(() =>
            {
                _valueChanged = true;
                StartSubscriptionCommand.RaiseCanExecuteChanged();                

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

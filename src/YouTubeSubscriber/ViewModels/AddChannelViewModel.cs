using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using YouTubeSubscriber.Models;
using YouTubeSubscriber.Services;

namespace YouTubeSubscriber.ViewModels
{
    public class AddChannelViewModel : BindableBase
    {
        private string _urlField;
        private string _statusText;
        private bool _isBusy;
        private bool _channelVerified;

        public bool IsBusy { get => _isBusy; set { SetProperty(ref _isBusy, value); } }
        public string StatusText { get => _statusText; set { SetProperty(ref _statusText, value); } }
        public string UrlField
        {
            get => _urlField;
            set
            {
                SetProperty(ref _urlField, value);
                VerifyChannelCommand.RaiseCanExecuteChanged();
            }
        }        
        

        public DelegateCommand VerifyChannelCommand { get; }
        public DelegateCommand AddChannelCommand { get; }      

        public AddChannelViewModel()
        {
            StatusText = "";

            VerifyChannelCommand = new DelegateCommand(() =>
            {
                Task.Run(() =>
                {
                    IsBusy = true;
                    StatusText += "Verifying channel...\n";

                    using (var automatization = new Automatization(true))
                    {
                        var channel = new Channel()
                        {
                            Url = UrlField,
                        };

                        if (automatization.VerifyChannel(ref channel))
                        {
                            StatusText += "Channel verified now you can add to list\n";
                            StatusText += channel.ToString() + "\n";
                            _channelVerified = true;
                            IsBusy = false;
                            AddChannelCommand.RaiseCanExecuteChanged();
                        }
                    }                    
                });
                
            }, CanExecuteVerifyCommand);

            AddChannelCommand = new DelegateCommand(() =>
            {
                UrlField = "";
                _channelVerified = false;
                AddChannelCommand.RaiseCanExecuteChanged();

            }, CanExecuteAddCommand);
        }

        private bool CanExecuteVerifyCommand() => !string.IsNullOrWhiteSpace(UrlField);
        private bool CanExecuteAddCommand() => _channelVerified;
    }
}

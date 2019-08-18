using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using YouTubeSubscriber.Data;
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
        private readonly ApplicationDbContext _context;
        private Channel _channel;

        public ObservableCollection<Channel> Channels { get; set; }
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

        public AddChannelViewModel(ApplicationDbContext context)
        {
            _context = context;
            StatusText = "";

            VerifyChannelCommand = new DelegateCommand(() =>
            {
                Task.Run(() =>
                {
                    IsBusy = true;
                    StatusText += "Verifying channel...\n";
                    _channel = new Channel()
                    {
                        Url = UrlField,
                    };

                    using (var subscriberService = new ChannelSubscriberService(_context, _channel, true))
                    {
                        if (subscriberService.VerifyChannel())
                        {
                            StatusText += "Channel verified now you can add to database!\n";
                            StatusText += _channel.ToString() + "\n";
                            _channelVerified = true;
                            IsBusy = false;
                            AddChannelCommand.RaiseCanExecuteChanged();
                        }
                    }                    
                });
                
            }, CanExecuteVerifyCommand);

            AddChannelCommand = new DelegateCommand(() =>
            {
                _channelVerified = false;
                
                var channelFromDb = _context.Channels.Where(i => i.Url == _channel.Url).FirstOrDefault();

                if (channelFromDb != null)
                {
                    StatusText += "ERROR: Channel already exists in database\n";
                    return;
                }
                else
                {
                    _context.Channels.Add(_channel);
                    _context.SaveChanges();
                    Channels.Add(_channel);
                    StatusText += $"Added channel to database \n{_channel}";
                    UrlField = "";
                    AddChannelCommand.RaiseCanExecuteChanged();
                }               

            }, CanExecuteAddCommand);
        }

        private bool CanExecuteVerifyCommand() => !string.IsNullOrWhiteSpace(UrlField);
        private bool CanExecuteAddCommand() => _channelVerified;
    }
}

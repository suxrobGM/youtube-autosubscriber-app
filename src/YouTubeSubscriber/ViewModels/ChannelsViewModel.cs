using System.Collections.ObjectModel;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using YouTubeSubscriber.Data;
using YouTubeSubscriber.Models;
using YouTubeSubscriber.Views;

namespace YouTubeSubscriber.ViewModels
{
    public class ChannelsViewModel : BindableBase
    {
        private readonly ApplicationDbContext _context;
        private Channel _selectedChannel;
            
        public ObservableCollection<Channel> Channels { get; }
        public DelegateCommand AddChannelCommand { get; }
        public DelegateCommand EditSubscribersCommand { get; }
        public DelegateCommand RemoveChannel { get; }
        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                SetProperty(ref _selectedChannel, value);
                EditSubscribersCommand.RaiseCanExecuteChanged();
                RemoveChannel.RaiseCanExecuteChanged();
            }
        }

        public ChannelsViewModel(ApplicationDbContext context)
        {
            _context = context;
            Channels = new ObservableCollection<Channel>(_context.Channels);
            //GenerateChannels();

            AddChannelCommand = new DelegateCommand(() =>
            {
                var addChannelView = new AddChannelView();
                (addChannelView.DataContext as AddChannelViewModel).Channels = Channels;
                addChannelView.ShowDialog();
            });

            EditSubscribersCommand = new DelegateCommand(() =>
            {
                var editSubscribersView = new EditChannelSubscribersView();
                (editSubscribersView.DataContext as EditChannelSubscribersViewModel).Channel = SelectedChannel;
                editSubscribersView.ShowDialog();

            }, CanExecuteEditSubscribers);

            RemoveChannel = new DelegateCommand(() =>
            {
                var msgResult = MessageBox.Show($"Do you want to remove channel from database?\n{SelectedChannel}", "Remove channel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (msgResult == DialogResult.Yes)
                {
                    _context.Channels.Remove(SelectedChannel);
                    _context.SaveChanges();
                    Channels.Remove(SelectedChannel);
                }

            }, CanExecuteEditSubscribers);
        }

        private bool CanExecuteEditSubscribers() => SelectedChannel != null;
        private void GenerateChannels()
        {
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

            foreach (var item in channels)
            {
                Channels.Add(item);
            }
        }
    }
}

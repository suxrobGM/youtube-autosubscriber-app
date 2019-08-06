using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using YouTubeSubscriber.Models;
using YouTubeSubscriber.Views;

namespace YouTubeSubscriber.ViewModels
{
    public class ChannelsViewModel : BindableBase
    {
        private Channel _selectedChannel;
            
        public ObservableCollection<Channel> Channels { get; }
        public DelegateCommand AddChannelCommand { get; }
        public DelegateCommand EditSubscribersCommand { get; }
        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                SetProperty(ref _selectedChannel, value);
                EditSubscribersCommand.RaiseCanExecuteChanged();
            }
        }

        public ChannelsViewModel()
        {
            Channels = new ObservableCollection<Channel>();
            GenerateChannels();

            AddChannelCommand = new DelegateCommand(() =>
            {
                var addChannelView = new AddChannelView();
                addChannelView.ShowDialog();
            });

            EditSubscribersCommand = new DelegateCommand(() =>
            {
                var editSubscribersView = new EditChannelSubscribersView();
                (editSubscribersView.DataContext as EditChannelSubscribersViewModel).Channel = SelectedChannel;
                editSubscribersView.ShowDialog();

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

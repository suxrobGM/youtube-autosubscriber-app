using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.ViewModels
{
    public class ChannelsViewModel : BindableBase
    {
        private Channel _selectedChannel;

        public ObservableCollection<Channel> Channels { get; }
        public Channel SelectedChannel { get => _selectedChannel; set { SetProperty(ref _selectedChannel, value); } }

        public ChannelsViewModel()
        {
            Channels = new ObservableCollection<Channel>();
            GenerateChannels();
        }

        private void GenerateChannels()
        {
            var channels = new Channel[]
            {
                new Channel()
                {
                    Name = "UzbekFilmsHD",
                    Url = "https://www.youtube.com/user/UzbekFilmsHD",
                },
                new Channel()
                {
                    Name = "Dumbazz",
                    Url = "https://www.youtube.com/channel/UC64Dw03B-EgwrNERMOy1u5w",
                },
                new Channel()
                {
                    Name = "NevoFilms",
                    Url = "https://www.youtube.com/channel/UC8O6rvkAJqGVWMlT9zgdH9Q",
                },
                new Channel()
                {
                    Name = "KinoCheck International",
                    Url = "https://www.youtube.com/user/Filme",
                },
                new Channel()
                {
                    Name = "Fast & Furious",
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

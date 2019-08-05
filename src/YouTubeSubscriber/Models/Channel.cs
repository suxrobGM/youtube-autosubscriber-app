using System;
using System.Collections.Generic;

namespace YouTubeSubscriber.Models
{
    public class Channel
    {
        public Channel()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            SubscribedAccounts = new List<ChannelAccount>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public virtual List<ChannelAccount> SubscribedAccounts { get; set; }
    }
}
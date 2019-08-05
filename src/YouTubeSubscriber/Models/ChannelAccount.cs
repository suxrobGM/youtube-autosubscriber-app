using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriber.Models
{
    public class ChannelAccount
    {
        public string ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public string AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}

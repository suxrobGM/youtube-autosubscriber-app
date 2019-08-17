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

        public override int GetHashCode()
        {
            return ChannelId.GetHashCode() + AccountId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var channelAccount = obj as ChannelAccount;
            return channelAccount.AccountId == AccountId && channelAccount.ChannelId == ChannelId;
        }
    }
}

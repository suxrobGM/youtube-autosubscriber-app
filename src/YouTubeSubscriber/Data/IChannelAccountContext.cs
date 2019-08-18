using Microsoft.EntityFrameworkCore;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.Data
{
    public interface IChannelAccountContext
    {
        DbSet<Account> Accounts { get; set; }
        DbSet<Channel> Channels { get; set; }
    }
}

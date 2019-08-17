using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Mvvm;
using YouTubeSubscriber.Data;

namespace YouTubeSubscriber.Models
{
    public class Account : BindableBase
    {
        private string _email;
        private string _password;
        private bool _isVerified;

        public Account()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            SubscribedChannels = new List<ChannelAccount>();
        }

        public string Id { get; set; }
        public string Email { get => _email; set { SetProperty(ref _email, value); } }
        public string Password { get => _password; set { SetProperty(ref _password, value); } }
        public bool IsVerified { get => _isVerified; set { SetProperty(ref _isVerified, value); } }
        public virtual List<ChannelAccount> SubscribedChannels { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Account detail:");
            builder.AppendLine($" Email: {Email}");
            builder.AppendLine($" Password: {Password}");
            builder.AppendLine($" IsVerified: {IsVerified}");

            return builder.ToString();
        }

        public static Account[] GetUnsubcribedAccounts(ApplicationDbContext context, Channel channel, int count)
        {           
            var accounts = context.Accounts.Where(i => !i.SubscribedChannels.Where(x => x.ChannelId == channel.Id).Any()).Take(count);

            return accounts.ToArray();
        }

        public static Account[] GetSubcribedAccounts(ApplicationDbContext context, Channel channel, int count)
        {
            var accounts = context.Accounts.Where(i => i.SubscribedChannels.Where(x => x.ChannelId == channel.Id).Any()).Take(count);

            return accounts.ToArray();
        }
    }
}

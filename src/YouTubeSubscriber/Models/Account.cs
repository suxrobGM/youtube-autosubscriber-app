﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriber.Models
{
    public class Account
    {
        public Account()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            SubscribedChannels = new List<ChannelAccount>();
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsVerified { get; set; }
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
    }
}

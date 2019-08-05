using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
        public string Title { get; set; }
        public string Url { get; set; }
        public long SubscriberCount { get; set; }
        public virtual List<ChannelAccount> SubscribedAccounts { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Channel detail:");
            builder.AppendLine($"  Title: {Title}");
            builder.AppendLine($"  SubscriberCount: {SubscriberCount}");
            builder.AppendLine($"  Url: {Url}");

            return builder.ToString();
        }

        public static long ParseSubscriberCount(string subscriberCountString)
        {
            subscriberCountString = subscriberCountString.Replace(" ", "").Replace(",", "");
            var resultString = Regex.Match(subscriberCountString, @"\d+").Value;

            return long.Parse(resultString);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Prism.Mvvm;

namespace YouTubeSubscriber.Models
{
    public class Channel : BindableBase
    {
        private string _title;
        private string _url;
        private long _subscriberCount;

        public Channel()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            SubscribedAccounts = new List<ChannelAccount>();
        }

        public string Id { get; set; }
        public string Title { get => _title; set { SetProperty(ref _title, value); } }
        public string Url { get => _url; set { SetProperty(ref _url, value); } }
        public long SubscriberCount { get => _subscriberCount; set { SetProperty(ref _subscriberCount, value); } }
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
﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YouTubeSubscriber.Data;

namespace YouTubeSubscriber.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190807084516_added_unique_indexes")]
    partial class added_unique_indexes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("YouTubeSubscriber.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<bool>("IsVerified");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("YouTubeSubscriber.Models.Channel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("SubscriberCount");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("YouTubeSubscriber.Models.ChannelAccount", b =>
                {
                    b.Property<string>("AccountId");

                    b.Property<string>("ChannelId");

                    b.HasKey("AccountId", "ChannelId");

                    b.HasIndex("ChannelId");

                    b.ToTable("ChannelAccount");
                });

            modelBuilder.Entity("YouTubeSubscriber.Models.ChannelAccount", b =>
                {
                    b.HasOne("YouTubeSubscriber.Models.Account", "Account")
                        .WithMany("SubscribedChannels")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("YouTubeSubscriber.Models.Channel", "Channel")
                        .WithMany("SubscribedAccounts")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.SetNull);
                });
#pragma warning restore 612, 618
        }
    }
}

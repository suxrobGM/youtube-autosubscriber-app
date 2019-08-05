using Microsoft.EntityFrameworkCore;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data source=app_db.sqlite");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChannelAccount>(entity =>
            {
                entity.HasKey(k => new { k.AccountId, k.ChannelId });

                entity.HasOne(m => m.Account)
                    .WithMany(m => m.SubscribedChannels)
                    .HasForeignKey(m => m.AccountId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(m => m.Channel)
                    .WithMany(m => m.SubscribedAccounts)
                    .HasForeignKey(m => m.ChannelId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SellerBox.Models.Database;

namespace SellerBox.Common
{
    public class DatabaseContext : IdentityDbContext<Users>
    {
        public DbSet<Logs> Logs { get; set; }

        public DbSet<Groups> Groups { get; set; }
        public DbSet<GroupAdmins> GroupAdmins { get; set; }

        public DbSet<VkCallbackMessages> VkCallbackMessages { get; set; }

        public DbSet<VkUsers> VkUsers { get; set; }
        public DbSet<Subscribers> Subscribers { get; set; }

        public DbSet<Messages> Messages { get; set; }
        public DbSet<FilesInMessage> FilesInMessage { get; set; }
        public DbSet<Files> Files { get; set; }

        public DbSet<Segments> Segments { get; set; }
        public DbSet<SubscribersInSegments> SubscribersInSegments { get; set; }

        public DbSet<WallPosts> WallPosts { get; set; }
        public DbSet<RepostScenarios> RepostScenarios { get; set; }
        public DbSet<SubscriberReposts> SubscriberReposts { get; set; }
        public DbSet<CheckedSubscribersInRepostScenarios> CheckedSubscribersInRepostScenarios { get; set; }

        public DbSet<Scenarios> Scenarios { get; set; }

        public DbSet<Chains> Chains { get; set; }
        public DbSet<ChainContents> ChainContents { get; set; }
        public DbSet<SubscribersInChains> SubscribersInChains { get; set; }

        public DbSet<ChatScenarios> ChatScenarios { get; set; }
        public DbSet<ChatScenarioContents> ChatScenarioContents { get; set; }
        public DbSet<SubscriberChatReplies> SubscriberChatReplies { get; set; }
        public DbSet<SubscribersInChatProgress> SubscribersInChatProgress { get; set; }

        public DbSet<BirthdayScenarios> BirthdayScenarios { get; set; }

        public DbSet<BirthdayWallScenarios> BirthdayWallScenarios { get; set; }

        public DbSet<History_BirthdayWall> History_BirthdayWall { get; set; }
        public DbSet<History_Birthday> History_Birthday { get; set; }
        public DbSet<History_GroupActions> History_GroupActions { get; set; }
        public DbSet<History_Messages> History_Messages { get; set; }
        public DbSet<History_Scenarios> History_Scenarios { get; set; }
        public DbSet<History_ShortUrlClicks> History_ShortUrlClicks { get; set; }
        public DbSet<History_SubscribersInChainSteps> History_SubscribersInChainSteps { get; set; }
        public DbSet<History_SubscribersInChatScenariosContents> History_SubscribersInChatScenariosContents { get; set; }
        public DbSet<History_Synchronization> SyncHistory { get; set; }
        public DbSet<History_WallPosts> History_WallPosts { get; set; }

        public DbSet<Scheduler_Messaging> Scheduler_Messaging { get; set; }

        public DbSet<Notifications> Notifications { get; set; }

        public DbSet<ShortUrls> ShortUrls { get; set; }

        public DatabaseContext()
        {
            Database.SetCommandTimeout(int.MaxValue);
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.SetCommandTimeout(int.MaxValue);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VkUsers>()
            .HasKey(c => c.IdVk);

            modelBuilder.Entity<VkUsers>()
                .Property(c => c.IdVk)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Groups>()
            .HasKey(c => c.IdVk);

            modelBuilder.Entity<Groups>()
                .Property(c => c.IdVk)
                .ValueGeneratedNever();

            modelBuilder.Entity<VkCallbackMessages>()
                .Property(b => b.IsProcessed)
                .HasDefaultValue(true);

            modelBuilder.Entity<SubscriberReposts>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscriberReposts)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscriberChatReplies>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscriberChatReplies)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscribersInChains>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscribersInChains)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscribersInSegments>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscribersInSegments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscribersInChatProgress>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscribersInChatProgress)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BirthdayScenarios>()
                .HasOne(x => x.Group)
                .WithMany(x => x.BirthdayScenarios)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BirthdayWallScenarios>()
                .HasOne(x => x.Group)
                .WithMany(x => x.BirthdayWallScenarios)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_Birthday>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_Birthday)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_BirthdayWall>()
                .HasOne(x => x.WallPost)
                .WithOne(x => x.BirthdayWallHistory)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_GroupActions>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_GroupActions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_Messages>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_Messages)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_Scenarios>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_Scenarios)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_ShortUrlClicks>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_ShortUrlClicks)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_SubscribersInChainSteps>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_SubscribersInChainSteps)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_SubscribersInChatScenariosContents>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_SubscribersInChatScenariosContents)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History_WallPosts>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.History_WallPosts)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<CheckedSubscribersInRepostScenarios>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.CheckedSubscribersInRepostScenarios)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

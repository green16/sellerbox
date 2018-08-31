using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Database;

namespace WebApplication1.Common
{
    public class DatabaseContext : IdentityDbContext<Users>
    {
        public DbSet<Logs> Logs { get; set; }

        public DbSet<Groups> Groups { get; set; }
        public DbSet<GroupAdmins> GroupAdmins { get; set; }

        public DbSet<VkCallbackMessages> VkCallbackMessages { get; set; }

        public DbSet<VkUsers> VkUsers { get; set; }
        public DbSet<Subscribers> Subscribers { get; set; }

        public DbSet<MessagesGroups> MessagesGroups { get; set; }
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

        public DbSet<BirthdayScenarios> BirthdayScenarios { get; set; }
        public DbSet<BirthdayHistory> BirthdayHistory { get; set; }

        public DbSet<BirthdayWallScenarios> BirthdayWallScenarios { get; set; }

        public DbSet<SyncHistory> SyncHistory { get; set; }
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


            modelBuilder.Entity<SubscriberReposts>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscriberReposts)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscribersInChains>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscribersInChains)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscribersInSegments>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.SubscribersInSegments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BirthdayScenarios>()
                .HasOne(x => x.Group)
                .WithMany(x => x.BirthdayScenarios)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BirthdayWallScenarios>()
                .HasOne(x => x.Group)
                .WithMany(x => x.BirthdayWallScenarios)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckedSubscribersInRepostScenarios>()
                .HasOne(x => x.Subscriber)
                .WithMany(x => x.CheckedSubscribersInRepostScenarios)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

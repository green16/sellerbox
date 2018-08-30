﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Common;

namespace WebApplication1.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.BirthdayHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtSend")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGroup");

                    b.Property<int>("IdVkUser");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.HasIndex("IdVkUser");

                    b.ToTable("BirthdayHistory");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.BirthdayScenarios", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DaysBefore");

                    b.Property<int>("IdGroup");

                    b.Property<Guid>("IdMessage");

                    b.Property<bool>("IsEnabled");

                    b.Property<byte>("SendAt");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.HasIndex("IdMessage");

                    b.ToTable("BirthdayScenarios");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.ChainContents", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("IdChain");

                    b.Property<Guid?>("IdExcludeFromChain");

                    b.Property<Guid?>("IdGoToChain");

                    b.Property<Guid?>("IdMessage");

                    b.Property<int>("Index");

                    b.Property<bool>("IsOnlyDayTime");

                    b.Property<int>("SendAfterSeconds");

                    b.HasKey("Id");

                    b.HasIndex("IdChain");

                    b.HasIndex("IdExcludeFromChain");

                    b.HasIndex("IdGoToChain");

                    b.HasIndex("IdMessage");

                    b.ToTable("ChainContents");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Chains", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IdGroup");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.ToTable("Chains");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.CheckedSubscribersInRepostScenarios", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtCheck");

                    b.Property<Guid?>("IdRepostScenario");

                    b.Property<Guid>("IdSubscriber");

                    b.HasKey("Id");

                    b.HasIndex("IdRepostScenario");

                    b.HasIndex("IdSubscriber");

                    b.ToTable("CheckedSubscribersInRepostScenarios");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Files", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Content");

                    b.Property<DateTime>("DtAdd")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name");

                    b.Property<long>("Size");

                    b.Property<string>("VkUrl");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.FilesInMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("IdFile");

                    b.Property<Guid>("IdMessage");

                    b.HasKey("Id");

                    b.HasIndex("IdFile");

                    b.HasIndex("IdMessage");

                    b.ToTable("FilesInMessage");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.GroupAdmins", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtConnect")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGroup");

                    b.Property<string>("IdUser");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.HasIndex("IdUser");

                    b.ToTable("GroupAdmins");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Groups", b =>
                {
                    b.Property<int>("IdVk");

                    b.Property<string>("AccessToken");

                    b.Property<string>("CallbackConfirmationCode");

                    b.Property<string>("Name");

                    b.Property<string>("Photo");

                    b.HasKey("IdVk");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Logs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Callsite");

                    b.Property<string>("Exception");

                    b.Property<byte>("Level");

                    b.Property<DateTime>("Logged")
                        .HasColumnType("datetime2");

                    b.Property<string>("Logger")
                        .HasMaxLength(50);

                    b.Property<string>("Message");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Messages", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IdGroup");

                    b.Property<bool>("IsImageFirst");

                    b.Property<string>("Keyboard");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.RepostScenarios", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CheckAfterSeconds");

                    b.Property<bool>("CheckAllPosts");

                    b.Property<bool>("CheckLastPosts");

                    b.Property<Guid>("IdCheckingChainContent");

                    b.Property<Guid?>("IdGoToChain");

                    b.Property<Guid?>("IdGoToChain2");

                    b.Property<Guid?>("IdPost");

                    b.Property<bool>("IsEnabled");

                    b.Property<int?>("LastPostsCount");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("IdCheckingChainContent");

                    b.HasIndex("IdGoToChain");

                    b.HasIndex("IdGoToChain2");

                    b.HasIndex("IdPost");

                    b.ToTable("RepostScenarios");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Scenarios", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("Action");

                    b.Property<Guid?>("IdChain");

                    b.Property<Guid?>("IdChain2");

                    b.Property<Guid?>("IdErrorMessage");

                    b.Property<int>("IdGroup");

                    b.Property<Guid?>("IdMessage");

                    b.Property<string>("InputMessage");

                    b.Property<bool>("IsEnabled");

                    b.Property<bool>("IsStrictMatch");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("IdChain");

                    b.HasIndex("IdChain2");

                    b.HasIndex("IdErrorMessage");

                    b.HasIndex("IdGroup");

                    b.HasIndex("IdMessage");

                    b.ToTable("Scenarios");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Segments", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtCreate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGroup");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.ToTable("Segments");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.SubscriberReposts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtRepost")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IdPost");

                    b.Property<Guid>("IdSubscriber");

                    b.Property<bool>("IsProcessed");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("IdPost");

                    b.HasIndex("IdSubscriber");

                    b.ToTable("SubscriberReposts");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Subscribers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtAdd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DtUnsubscribe")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGroup");

                    b.Property<int>("IdVkUser");

                    b.Property<bool>("IsBlocked");

                    b.Property<bool?>("IsChatAllowed");

                    b.Property<bool?>("IsSubscribedToGroup");

                    b.Property<bool>("IsUnsubscribed");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.HasIndex("IdVkUser");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.SubscribersInChains", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtAdd")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IdChainStep");

                    b.Property<Guid>("IdSubscriber");

                    b.Property<bool>("IsSended");

                    b.HasKey("Id");

                    b.HasIndex("IdChainStep");

                    b.HasIndex("IdSubscriber");

                    b.ToTable("SubscribersInChains");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.SubscribersInSegments", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtAdd")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IdSegment");

                    b.Property<Guid>("IdSubscriber");

                    b.HasKey("Id");

                    b.HasIndex("IdSegment");

                    b.HasIndex("IdSubscriber");

                    b.ToTable("SubscribersInSegments");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.SyncHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DtEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DtStart")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGroup");

                    b.Property<byte>("SyncType");

                    b.HasKey("Id");

                    b.ToTable("SyncHistory");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Users", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int>("IdCurrentGroup");

                    b.Property<int>("IdVk");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.VkCallbackMessages", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Dt")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGroup");

                    b.Property<string>("Object");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("VkCallbackMessages");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.VkUsers", b =>
                {
                    b.Property<int>("IdVk");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Link");

                    b.Property<string>("PhotoOrig400");

                    b.Property<string>("PhotoSquare50");

                    b.Property<string>("SecondName");

                    b.Property<bool?>("Sex");

                    b.HasKey("IdVk");

                    b.ToTable("VkUsers");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.WallPosts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DtAdd")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGroup");

                    b.Property<int>("IdVk");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.ToTable("WallPosts");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Users")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Users")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Users")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Users")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.BirthdayHistory", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.VkUsers", "VkUser")
                        .WithMany()
                        .HasForeignKey("IdVkUser")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.BirthdayScenarios", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany("BirthdayScenarios")
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("WebApplication1.Models.Database.Messages", "Message")
                        .WithMany()
                        .HasForeignKey("IdMessage")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.ChainContents", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Chains", "Chain")
                        .WithMany()
                        .HasForeignKey("IdChain")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Chains", "ExcludeFromChain")
                        .WithMany()
                        .HasForeignKey("IdExcludeFromChain");

                    b.HasOne("WebApplication1.Models.Database.Chains", "GoToChain")
                        .WithMany()
                        .HasForeignKey("IdGoToChain");

                    b.HasOne("WebApplication1.Models.Database.Messages", "Message")
                        .WithMany()
                        .HasForeignKey("IdMessage");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Chains", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.CheckedSubscribersInRepostScenarios", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.RepostScenarios", "RepostScenario")
                        .WithMany()
                        .HasForeignKey("IdRepostScenario");

                    b.HasOne("WebApplication1.Models.Database.Subscribers", "Subscriber")
                        .WithMany("CheckedSubscribersInRepostScenarios")
                        .HasForeignKey("IdSubscriber")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.FilesInMessage", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Files", "File")
                        .WithMany()
                        .HasForeignKey("IdFile")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Messages", "Message")
                        .WithMany("Files")
                        .HasForeignKey("IdMessage")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.GroupAdmins", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany("GroupAdmins")
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Users", "User")
                        .WithMany()
                        .HasForeignKey("IdUser");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Messages", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.RepostScenarios", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.ChainContents", "CheckingChainContent")
                        .WithMany()
                        .HasForeignKey("IdCheckingChainContent")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Chains", "GoToChain")
                        .WithMany()
                        .HasForeignKey("IdGoToChain");

                    b.HasOne("WebApplication1.Models.Database.Chains", "GoToChain2")
                        .WithMany()
                        .HasForeignKey("IdGoToChain2");

                    b.HasOne("WebApplication1.Models.Database.WallPosts", "WallPost")
                        .WithMany()
                        .HasForeignKey("IdPost");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Scenarios", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Chains", "Chain")
                        .WithMany()
                        .HasForeignKey("IdChain");

                    b.HasOne("WebApplication1.Models.Database.Chains", "Chain2")
                        .WithMany()
                        .HasForeignKey("IdChain2");

                    b.HasOne("WebApplication1.Models.Database.Messages", "ErrorMessage")
                        .WithMany()
                        .HasForeignKey("IdErrorMessage");

                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Messages", "Message")
                        .WithMany()
                        .HasForeignKey("IdMessage");
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Segments", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.SubscriberReposts", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.WallPosts", "WallPost")
                        .WithMany()
                        .HasForeignKey("IdPost")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Subscribers", "Subscriber")
                        .WithMany("SubscriberReposts")
                        .HasForeignKey("IdSubscriber")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.Subscribers", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany("Subscribers")
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.VkUsers", "VkUser")
                        .WithMany()
                        .HasForeignKey("IdVkUser")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.SubscribersInChains", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.ChainContents", "ChainStep")
                        .WithMany()
                        .HasForeignKey("IdChainStep")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Subscribers", "Subscriber")
                        .WithMany("SubscribersInChains")
                        .HasForeignKey("IdSubscriber")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.SubscribersInSegments", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Segments", "Segment")
                        .WithMany()
                        .HasForeignKey("IdSegment")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication1.Models.Database.Subscribers", "Subscriber")
                        .WithMany("SubscribersInSegments")
                        .HasForeignKey("IdSubscriber")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebApplication1.Models.Database.WallPosts", b =>
                {
                    b.HasOne("WebApplication1.Models.Database.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

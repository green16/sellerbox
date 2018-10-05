using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IdVk = table.Column<long>(nullable: false),
                    IdCurrentGroup = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Size = table.Column<long>(nullable: false),
                    Content = table.Column<byte[]>(nullable: true),
                    VkUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    IdVk = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Photo = table.Column<string>(nullable: true),
                    AccessToken = table.Column<string>(nullable: true),
                    CallbackConfirmationCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.IdVk);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Logged = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<byte>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(maxLength: 50, nullable: true),
                    Callsite = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VkUsers",
                columns: table => new
                {
                    IdVk = table.Column<long>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    SecondName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    PhotoSquare50 = table.Column<string>(nullable: true),
                    PhotoOrig400 = table.Column<string>(nullable: true),
                    Sex = table.Column<bool>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkUsers", x => x.IdVk);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chains",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chains_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    InputMessage = table.Column<string>(nullable: true),
                    HasFormula = table.Column<bool>(nullable: false),
                    Formula = table.Column<string>(nullable: true),
                    IdGroup = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatScenarios_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    IdUser = table.Column<string>(nullable: true),
                    DtConnect = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAdmins_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAdmins_AspNetUsers_IdUser",
                        column: x => x.IdUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    IsImageFirst = table.Column<bool>(nullable: false),
                    Keyboard = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Segments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DtCreate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGroup = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Segments_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SyncHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    SyncType = table.Column<byte>(nullable: false),
                    DtStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncHistory_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkCallbackMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    Object = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkCallbackMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkCallbackMessages_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WallPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdVk = table.Column<long>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(nullable: true),
                    IdGroup = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WallPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WallPosts_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtUnsubscribe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsChatAllowed = table.Column<bool>(nullable: true),
                    IsSubscribedToGroup = table.Column<bool>(nullable: true),
                    IsUnsubscribed = table.Column<bool>(nullable: false),
                    IsBlocked = table.Column<bool>(nullable: false),
                    IdVkUser = table.Column<long>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscribers_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscribers_VkUsers_IdVkUser",
                        column: x => x.IdVkUser,
                        principalTable: "VkUsers",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatScenarioContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Step = table.Column<long>(nullable: false),
                    IdChatScenario = table.Column<Guid>(nullable: false),
                    IdMessage = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatScenarioContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatScenarioContents_ChatScenarios_IdChatScenario",
                        column: x => x.IdChatScenario,
                        principalTable: "ChatScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SendAt = table.Column<byte>(nullable: false),
                    DaysBefore = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    IdMessage = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayScenarios_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BirthdayScenarios_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayWallScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SendAt = table.Column<byte>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IdVkUser = table.Column<long>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    IdMessage = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayWallScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayWallScenarios_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BirthdayWallScenarios_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthdayWallScenarios_VkUsers_IdVkUser",
                        column: x => x.IdVkUser,
                        principalTable: "VkUsers",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChainContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    IsOnlyDayTime = table.Column<bool>(nullable: false),
                    SendAfterSeconds = table.Column<int>(nullable: false),
                    IdChain = table.Column<Guid>(nullable: false),
                    IdGoToChain = table.Column<Guid>(nullable: true),
                    IdExcludeFromChain = table.Column<Guid>(nullable: true),
                    IdMessage = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChainContents_Chains_IdChain",
                        column: x => x.IdChain,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChainContents_Chains_IdExcludeFromChain",
                        column: x => x.IdExcludeFromChain,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChainContents_Chains_IdGoToChain",
                        column: x => x.IdGoToChain,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChainContents_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilesInMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdMessage = table.Column<Guid>(nullable: false),
                    IdFile = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesInMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilesInMessage_Files_IdFile",
                        column: x => x.IdFile,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilesInMessage_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    InputMessage = table.Column<string>(nullable: true),
                    IsStrictMatch = table.Column<bool>(nullable: false),
                    Action = table.Column<byte>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    IdMessage = table.Column<Guid>(nullable: true),
                    IdErrorMessage = table.Column<Guid>(nullable: true),
                    IdChain = table.Column<Guid>(nullable: true),
                    IdChain2 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scenarios_Chains_IdChain",
                        column: x => x.IdChain,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Scenarios_Chains_IdChain2",
                        column: x => x.IdChain2,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Scenarios_Messages_IdErrorMessage",
                        column: x => x.IdErrorMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Scenarios_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scenarios_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Scheduler_Messaging",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IdMessage = table.Column<Guid>(nullable: false),
                    VkUserIds = table.Column<string>(nullable: true),
                    RecipientsCount = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scheduler_Messaging", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scheduler_Messaging_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History_BirthdayWall",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdVkUser = table.Column<long>(nullable: false),
                    IdPost = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    DtSend = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_BirthdayWall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_BirthdayWall_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_BirthdayWall_WallPosts_IdPost",
                        column: x => x.IdPost,
                        principalTable: "WallPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_History_BirthdayWall_VkUsers_IdVkUser",
                        column: x => x.IdVkUser,
                        principalTable: "VkUsers",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History_Birthday",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    DtSend = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_Birthday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Birthday_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_Birthday_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "History_GroupActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    ActionType = table.Column<byte>(nullable: false),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_GroupActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_GroupActions_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_GroupActions_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "History_Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdMessage = table.Column<Guid>(nullable: true),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IsOutgoingMessage = table.Column<bool>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Messages_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_History_Messages_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "History_WallPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsRepost = table.Column<bool>(nullable: false),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdPost = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_WallPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_WallPosts_WallPosts_IdPost",
                        column: x => x.IdPost,
                        principalTable: "WallPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_WallPosts_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberReposts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    IsProcessed = table.Column<bool>(nullable: false),
                    DtRepost = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdPost = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberReposts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriberReposts_WallPosts_IdPost",
                        column: x => x.IdPost,
                        principalTable: "WallPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberReposts_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscribersInSegments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdSegment = table.Column<Guid>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribersInSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribersInSegments_Segments_IdSegment",
                        column: x => x.IdSegment,
                        principalTable: "Segments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscribersInSegments_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "History_SubscribersInChatScenariosContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdChatScenarioContent = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_SubscribersInChatScenariosContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_SubscribersInChatScenariosContents_ChatScenarioContents_IdChatScenarioContent",
                        column: x => x.IdChatScenarioContent,
                        principalTable: "ChatScenarioContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_SubscribersInChatScenariosContents_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberChatReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UniqueId = table.Column<Guid>(nullable: false),
                    IdChatScenarioContent = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberChatReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriberChatReplies_ChatScenarioContents_IdChatScenarioContent",
                        column: x => x.IdChatScenarioContent,
                        principalTable: "ChatScenarioContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberChatReplies_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscribersInChatProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    IdChatScenarioContent = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribersInChatProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribersInChatProgress_ChatScenarioContents_IdChatScenarioContent",
                        column: x => x.IdChatScenarioContent,
                        principalTable: "ChatScenarioContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscribersInChatProgress_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "History_SubscribersInChainSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdChainStep = table.Column<Guid>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_SubscribersInChainSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_SubscribersInChainSteps_ChainContents_IdChainStep",
                        column: x => x.IdChainStep,
                        principalTable: "ChainContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_SubscribersInChainSteps_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepostScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    CheckAfterSeconds = table.Column<int>(nullable: false),
                    IdPost = table.Column<Guid>(nullable: true),
                    CheckLastPosts = table.Column<bool>(nullable: false),
                    LastPostsCount = table.Column<int>(nullable: true),
                    CheckAllPosts = table.Column<bool>(nullable: false),
                    IdCheckingChainContent = table.Column<Guid>(nullable: false),
                    IdGoToChain = table.Column<Guid>(nullable: true),
                    IdGoToChain2 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepostScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepostScenarios_ChainContents_IdCheckingChainContent",
                        column: x => x.IdCheckingChainContent,
                        principalTable: "ChainContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepostScenarios_Chains_IdGoToChain",
                        column: x => x.IdGoToChain,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepostScenarios_Chains_IdGoToChain2",
                        column: x => x.IdGoToChain2,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepostScenarios_WallPosts_IdPost",
                        column: x => x.IdPost,
                        principalTable: "WallPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscribersInChains",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdChainStep = table.Column<Guid>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSended = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribersInChains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribersInChains_ChainContents_IdChainStep",
                        column: x => x.IdChainStep,
                        principalTable: "ChainContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscribersInChains_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "History_Scenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdScenario = table.Column<Guid>(nullable: false),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_Scenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Scenarios_Scenarios_IdScenario",
                        column: x => x.IdScenario,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_Scenarios_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CheckedSubscribersInRepostScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdRepostScenario = table.Column<Guid>(nullable: true),
                    DtCheck = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckedSubscribersInRepostScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckedSubscribersInRepostScenarios_RepostScenarios_IdRepostScenario",
                        column: x => x.IdRepostScenario,
                        principalTable: "RepostScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckedSubscribersInRepostScenarios_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayScenarios_IdGroup",
                table: "BirthdayScenarios",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayScenarios_IdMessage",
                table: "BirthdayScenarios",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallScenarios_IdGroup",
                table: "BirthdayWallScenarios",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallScenarios_IdMessage",
                table: "BirthdayWallScenarios",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallScenarios_IdVkUser",
                table: "BirthdayWallScenarios",
                column: "IdVkUser");

            migrationBuilder.CreateIndex(
                name: "IX_ChainContents_IdChain",
                table: "ChainContents",
                column: "IdChain");

            migrationBuilder.CreateIndex(
                name: "IX_ChainContents_IdExcludeFromChain",
                table: "ChainContents",
                column: "IdExcludeFromChain");

            migrationBuilder.CreateIndex(
                name: "IX_ChainContents_IdGoToChain",
                table: "ChainContents",
                column: "IdGoToChain");

            migrationBuilder.CreateIndex(
                name: "IX_ChainContents_IdMessage",
                table: "ChainContents",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_Chains_IdGroup",
                table: "Chains",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_ChatScenarioContents_IdChatScenario",
                table: "ChatScenarioContents",
                column: "IdChatScenario");

            migrationBuilder.CreateIndex(
                name: "IX_ChatScenarios_IdGroup",
                table: "ChatScenarios",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_CheckedSubscribersInRepostScenarios_IdRepostScenario",
                table: "CheckedSubscribersInRepostScenarios",
                column: "IdRepostScenario");

            migrationBuilder.CreateIndex(
                name: "IX_CheckedSubscribersInRepostScenarios_IdSubscriber",
                table: "CheckedSubscribersInRepostScenarios",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_FilesInMessage_IdFile",
                table: "FilesInMessage",
                column: "IdFile");

            migrationBuilder.CreateIndex(
                name: "IX_FilesInMessage_IdMessage",
                table: "FilesInMessage",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdmins_IdGroup",
                table: "GroupAdmins",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdmins_IdUser",
                table: "GroupAdmins",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_History_Birthday_IdGroup",
                table: "History_Birthday",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_History_Birthday_IdSubscriber",
                table: "History_Birthday",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_BirthdayWall_IdGroup",
                table: "History_BirthdayWall",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_History_BirthdayWall_IdPost",
                table: "History_BirthdayWall",
                column: "IdPost",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_BirthdayWall_IdVkUser",
                table: "History_BirthdayWall",
                column: "IdVkUser");

            migrationBuilder.CreateIndex(
                name: "IX_History_GroupActions_IdGroup",
                table: "History_GroupActions",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_History_GroupActions_IdSubscriber",
                table: "History_GroupActions",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_Messages_IdMessage",
                table: "History_Messages",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_History_Messages_IdSubscriber",
                table: "History_Messages",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_Scenarios_IdScenario",
                table: "History_Scenarios",
                column: "IdScenario");

            migrationBuilder.CreateIndex(
                name: "IX_History_Scenarios_IdSubscriber",
                table: "History_Scenarios",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChainSteps_IdChainStep",
                table: "History_SubscribersInChainSteps",
                column: "IdChainStep");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChainSteps_IdSubscriber",
                table: "History_SubscribersInChainSteps",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChatScenariosContents_IdChatScenarioContent",
                table: "History_SubscribersInChatScenariosContents",
                column: "IdChatScenarioContent");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChatScenariosContents_IdSubscriber",
                table: "History_SubscribersInChatScenariosContents",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_WallPosts_IdPost",
                table: "History_WallPosts",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_History_WallPosts_IdSubscriber",
                table: "History_WallPosts",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IdGroup",
                table: "Messages",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_RepostScenarios_IdCheckingChainContent",
                table: "RepostScenarios",
                column: "IdCheckingChainContent");

            migrationBuilder.CreateIndex(
                name: "IX_RepostScenarios_IdGoToChain",
                table: "RepostScenarios",
                column: "IdGoToChain");

            migrationBuilder.CreateIndex(
                name: "IX_RepostScenarios_IdGoToChain2",
                table: "RepostScenarios",
                column: "IdGoToChain2");

            migrationBuilder.CreateIndex(
                name: "IX_RepostScenarios_IdPost",
                table: "RepostScenarios",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_IdChain",
                table: "Scenarios",
                column: "IdChain");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_IdChain2",
                table: "Scenarios",
                column: "IdChain2");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_IdErrorMessage",
                table: "Scenarios",
                column: "IdErrorMessage");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_IdGroup",
                table: "Scenarios",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_IdMessage",
                table: "Scenarios",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_Scheduler_Messaging_IdMessage",
                table: "Scheduler_Messaging",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_Segments_IdGroup",
                table: "Segments",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberChatReplies_IdChatScenarioContent",
                table: "SubscriberChatReplies",
                column: "IdChatScenarioContent");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberChatReplies_IdSubscriber",
                table: "SubscriberChatReplies",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberReposts_IdPost",
                table: "SubscriberReposts",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberReposts_IdSubscriber",
                table: "SubscriberReposts",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_IdGroup",
                table: "Subscribers",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_IdVkUser",
                table: "Subscribers",
                column: "IdVkUser");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInChains_IdChainStep",
                table: "SubscribersInChains",
                column: "IdChainStep");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInChains_IdSubscriber",
                table: "SubscribersInChains",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInChatProgress_IdChatScenarioContent",
                table: "SubscribersInChatProgress",
                column: "IdChatScenarioContent");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInChatProgress_IdSubscriber",
                table: "SubscribersInChatProgress",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInSegments_IdSegment",
                table: "SubscribersInSegments",
                column: "IdSegment");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInSegments_IdSubscriber",
                table: "SubscribersInSegments",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_SyncHistory_IdGroup",
                table: "SyncHistory",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_VkCallbackMessages_IdGroup",
                table: "VkCallbackMessages",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_WallPosts_IdGroup",
                table: "WallPosts",
                column: "IdGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BirthdayScenarios");

            migrationBuilder.DropTable(
                name: "BirthdayWallScenarios");

            migrationBuilder.DropTable(
                name: "CheckedSubscribersInRepostScenarios");

            migrationBuilder.DropTable(
                name: "FilesInMessage");

            migrationBuilder.DropTable(
                name: "GroupAdmins");

            migrationBuilder.DropTable(
                name: "History_Birthday");

            migrationBuilder.DropTable(
                name: "History_BirthdayWall");

            migrationBuilder.DropTable(
                name: "History_GroupActions");

            migrationBuilder.DropTable(
                name: "History_Messages");

            migrationBuilder.DropTable(
                name: "History_Scenarios");

            migrationBuilder.DropTable(
                name: "History_SubscribersInChainSteps");

            migrationBuilder.DropTable(
                name: "History_SubscribersInChatScenariosContents");

            migrationBuilder.DropTable(
                name: "History_WallPosts");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Scheduler_Messaging");

            migrationBuilder.DropTable(
                name: "SubscriberChatReplies");

            migrationBuilder.DropTable(
                name: "SubscriberReposts");

            migrationBuilder.DropTable(
                name: "SubscribersInChains");

            migrationBuilder.DropTable(
                name: "SubscribersInChatProgress");

            migrationBuilder.DropTable(
                name: "SubscribersInSegments");

            migrationBuilder.DropTable(
                name: "SyncHistory");

            migrationBuilder.DropTable(
                name: "VkCallbackMessages");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "RepostScenarios");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Scenarios");

            migrationBuilder.DropTable(
                name: "ChatScenarioContents");

            migrationBuilder.DropTable(
                name: "Segments");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "ChainContents");

            migrationBuilder.DropTable(
                name: "WallPosts");

            migrationBuilder.DropTable(
                name: "ChatScenarios");

            migrationBuilder.DropTable(
                name: "VkUsers");

            migrationBuilder.DropTable(
                name: "Chains");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}

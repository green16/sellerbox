using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class Add_Histories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirthdayHistory");

            migrationBuilder.DropTable(
                name: "BirthdayWallHistory");

            migrationBuilder.CreateTable(
                name: "History_Birthday",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdVkUser = table.Column<long>(nullable: false),
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
                        name: "FK_History_Birthday_VkUsers_IdVkUser",
                        column: x => x.IdVkUser,
                        principalTable: "VkUsers",
                        principalColumn: "IdVk",
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
                    IdMessage = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IsOutgoingMessage = table.Column<bool>(nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_Messages_Subscribers_IdSubscriber",
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

            migrationBuilder.CreateIndex(
                name: "IX_History_Birthday_IdGroup",
                table: "History_Birthday",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_History_Birthday_IdVkUser",
                table: "History_Birthday",
                column: "IdVkUser");

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
                name: "IX_History_SubscribersInChainSteps_IdChainStep",
                table: "History_SubscribersInChainSteps",
                column: "IdChainStep");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChainSteps_IdSubscriber",
                table: "History_SubscribersInChainSteps",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_WallPosts_IdPost",
                table: "History_WallPosts",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_History_WallPosts_IdSubscriber",
                table: "History_WallPosts",
                column: "IdSubscriber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History_Birthday");

            migrationBuilder.DropTable(
                name: "History_BirthdayWall");

            migrationBuilder.DropTable(
                name: "History_GroupActions");

            migrationBuilder.DropTable(
                name: "History_Messages");

            migrationBuilder.DropTable(
                name: "History_SubscribersInChainSteps");

            migrationBuilder.DropTable(
                name: "History_WallPosts");

            migrationBuilder.CreateTable(
                name: "BirthdayHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DtSend = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    IdVkUser = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayHistory_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthdayHistory_VkUsers_IdVkUser",
                        column: x => x.IdVkUser,
                        principalTable: "VkUsers",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayWallHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DtSend = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    IdPost = table.Column<Guid>(nullable: false),
                    IdVkUser = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayWallHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayWallHistory_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthdayWallHistory_WallPosts_IdPost",
                        column: x => x.IdPost,
                        principalTable: "WallPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BirthdayWallHistory_VkUsers_IdVkUser",
                        column: x => x.IdVkUser,
                        principalTable: "VkUsers",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayHistory_IdGroup",
                table: "BirthdayHistory",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayHistory_IdVkUser",
                table: "BirthdayHistory",
                column: "IdVkUser");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallHistory_IdGroup",
                table: "BirthdayWallHistory",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallHistory_IdPost",
                table: "BirthdayWallHistory",
                column: "IdPost",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallHistory_IdVkUser",
                table: "BirthdayWallHistory",
                column: "IdVkUser");
        }
    }
}

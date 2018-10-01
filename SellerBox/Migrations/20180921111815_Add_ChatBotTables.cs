using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Add_ChatBotTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Scheduler_Messaging",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdMessage = table.Column<Guid>(nullable: true),
                    DtStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scheduler_Messaging", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scheduler_Messaging_Messages_IdMessage",
                        column: x => x.IdMessage,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "History_SubscribersInChatScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdChatScenario = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_SubscribersInChatScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_SubscribersInChatScenarios_ChatScenarios_IdChatScenario",
                        column: x => x.IdChatScenario,
                        principalTable: "ChatScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_SubscribersInChatScenarios_Subscribers_IdSubscriber",
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

            migrationBuilder.CreateIndex(
                name: "IX_ChatScenarioContents_IdChatScenario",
                table: "ChatScenarioContents",
                column: "IdChatScenario");

            migrationBuilder.CreateIndex(
                name: "IX_ChatScenarios_IdGroup",
                table: "ChatScenarios",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChatScenarios_IdChatScenario",
                table: "History_SubscribersInChatScenarios",
                column: "IdChatScenario");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChatScenarios_IdSubscriber",
                table: "History_SubscribersInChatScenarios",
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
                name: "IX_Scheduler_Messaging_IdMessage",
                table: "Scheduler_Messaging",
                column: "IdMessage");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberChatReplies_IdChatScenarioContent",
                table: "SubscriberChatReplies",
                column: "IdChatScenarioContent");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberChatReplies_IdSubscriber",
                table: "SubscriberChatReplies",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInChatProgress_IdChatScenarioContent",
                table: "SubscribersInChatProgress",
                column: "IdChatScenarioContent");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInChatProgress_IdSubscriber",
                table: "SubscribersInChatProgress",
                column: "IdSubscriber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History_SubscribersInChatScenarios");

            migrationBuilder.DropTable(
                name: "History_SubscribersInChatScenariosContents");

            migrationBuilder.DropTable(
                name: "Scheduler_Messaging");

            migrationBuilder.DropTable(
                name: "SubscriberChatReplies");

            migrationBuilder.DropTable(
                name: "SubscribersInChatProgress");

            migrationBuilder.DropTable(
                name: "ChatScenarioContents");

            migrationBuilder.DropTable(
                name: "ChatScenarios");
        }
    }
}

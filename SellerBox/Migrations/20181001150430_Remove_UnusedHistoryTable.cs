using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Remove_UnusedHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History_SubscribersInChatScenarios");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChatScenarios_IdChatScenario",
                table: "History_SubscribersInChatScenarios",
                column: "IdChatScenario");

            migrationBuilder.CreateIndex(
                name: "IX_History_SubscribersInChatScenarios_IdSubscriber",
                table: "History_SubscribersInChatScenarios",
                column: "IdSubscriber");
        }
    }
}

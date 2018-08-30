using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class AddBirthday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BirthdayHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdVkUser = table.Column<int>(nullable: false),
                    IdGroup = table.Column<int>(nullable: false),
                    DtSend = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "BirthdayScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SendAt = table.Column<byte>(nullable: false),
                    DaysBefore = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IdGroup = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayHistory_IdGroup",
                table: "BirthdayHistory",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayHistory_IdVkUser",
                table: "BirthdayHistory",
                column: "IdVkUser");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayScenarios_IdGroup",
                table: "BirthdayScenarios",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayScenarios_IdMessage",
                table: "BirthdayScenarios",
                column: "IdMessage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirthdayHistory");

            migrationBuilder.DropTable(
                name: "BirthdayScenarios");
        }
    }
}

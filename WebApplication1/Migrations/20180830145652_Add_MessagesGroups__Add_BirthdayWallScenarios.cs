using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class Add_MessagesGroups__Add_BirthdayWallScenarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MessagesGroupsId",
                table: "Messages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MessagesGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IdGroup = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessagesGroups_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayWallScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SendAt = table.Column<byte>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IdGroup = table.Column<int>(nullable: false),
                    IdMessagesGroup = table.Column<Guid>(nullable: false)
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
                        name: "FK_BirthdayWallScenarios_MessagesGroups_IdMessagesGroup",
                        column: x => x.IdMessagesGroup,
                        principalTable: "MessagesGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessagesGroupsId",
                table: "Messages",
                column: "MessagesGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallScenarios_IdGroup",
                table: "BirthdayWallScenarios",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayWallScenarios_IdMessagesGroup",
                table: "BirthdayWallScenarios",
                column: "IdMessagesGroup");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesGroups_IdGroup",
                table: "MessagesGroups",
                column: "IdGroup");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MessagesGroups_MessagesGroupsId",
                table: "Messages",
                column: "MessagesGroupsId",
                principalTable: "MessagesGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MessagesGroups_MessagesGroupsId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "BirthdayWallScenarios");

            migrationBuilder.DropTable(
                name: "MessagesGroups");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessagesGroupsId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessagesGroupsId",
                table: "Messages");
        }
    }
}

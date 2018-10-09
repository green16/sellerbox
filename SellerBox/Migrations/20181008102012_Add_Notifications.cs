using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Add_Notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DtStart",
                table: "SyncHistory",
                newName: "Dt");

            migrationBuilder.RenameColumn(
                name: "DtAdd",
                table: "History_SubscribersInChainSteps",
                newName: "Dt");

            migrationBuilder.RenameColumn(
                name: "DtSend",
                table: "History_BirthdayWall",
                newName: "Dt");

            migrationBuilder.RenameColumn(
                name: "DtSend",
                table: "History_Birthday",
                newName: "Dt");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    DtCreate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdElement = table.Column<string>(nullable: true),
                    ElementType = table.Column<int>(nullable: false),
                    NotifyTo = table.Column<string>(nullable: true),
                    NotificationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IdGroup",
                table: "Notifications",
                column: "IdGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Dt",
                table: "SyncHistory",
                newName: "DtStart");

            migrationBuilder.RenameColumn(
                name: "Dt",
                table: "History_SubscribersInChainSteps",
                newName: "DtAdd");

            migrationBuilder.RenameColumn(
                name: "Dt",
                table: "History_BirthdayWall",
                newName: "DtSend");

            migrationBuilder.RenameColumn(
                name: "Dt",
                table: "History_Birthday",
                newName: "DtSend");
        }
    }
}

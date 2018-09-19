using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class Update_Histories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_Birthday_VkUsers_IdVkUser",
                table: "History_Birthday");

            migrationBuilder.DropForeignKey(
                name: "FK_History_Messages_Messages_IdMessage",
                table: "History_Messages");

            migrationBuilder.DropIndex(
                name: "IX_History_Birthday_IdVkUser",
                table: "History_Birthday");

            migrationBuilder.DropColumn(
                name: "IdVkUser",
                table: "History_Birthday");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdMessage",
                table: "History_Messages",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "History_Messages",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdSubscriber",
                table: "History_Birthday",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.CreateIndex(
                name: "IX_History_Birthday_IdSubscriber",
                table: "History_Birthday",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_History_Scenarios_IdScenario",
                table: "History_Scenarios",
                column: "IdScenario");

            migrationBuilder.CreateIndex(
                name: "IX_History_Scenarios_IdSubscriber",
                table: "History_Scenarios",
                column: "IdSubscriber");

            migrationBuilder.AddForeignKey(
                name: "FK_History_Birthday_Subscribers_IdSubscriber",
                table: "History_Birthday",
                column: "IdSubscriber",
                principalTable: "Subscribers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_History_Messages_Messages_IdMessage",
                table: "History_Messages",
                column: "IdMessage",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_Birthday_Subscribers_IdSubscriber",
                table: "History_Birthday");

            migrationBuilder.DropForeignKey(
                name: "FK_History_Messages_Messages_IdMessage",
                table: "History_Messages");

            migrationBuilder.DropTable(
                name: "History_Scenarios");

            migrationBuilder.DropIndex(
                name: "IX_History_Birthday_IdSubscriber",
                table: "History_Birthday");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "History_Messages");

            migrationBuilder.DropColumn(
                name: "IdSubscriber",
                table: "History_Birthday");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdMessage",
                table: "History_Messages",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdVkUser",
                table: "History_Birthday",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_History_Birthday_IdVkUser",
                table: "History_Birthday",
                column: "IdVkUser");

            migrationBuilder.AddForeignKey(
                name: "FK_History_Birthday_VkUsers_IdVkUser",
                table: "History_Birthday",
                column: "IdVkUser",
                principalTable: "VkUsers",
                principalColumn: "IdVk",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_History_Messages_Messages_IdMessage",
                table: "History_Messages",
                column: "IdMessage",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class Fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scheduler_Messaging_Messages_IdMessage",
                table: "Scheduler_Messaging");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdMessage",
                table: "Scheduler_Messaging",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Scheduler_Messaging_Messages_IdMessage",
                table: "Scheduler_Messaging",
                column: "IdMessage",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scheduler_Messaging_Messages_IdMessage",
                table: "Scheduler_Messaging");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdMessage",
                table: "Scheduler_Messaging",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Scheduler_Messaging_Messages_IdMessage",
                table: "Scheduler_Messaging",
                column: "IdMessage",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class MessagingScheduler_AddExternalFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DtStart",
                table: "Scheduler_Messaging",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "DtAdd",
                table: "Scheduler_Messaging",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DtEnd",
                table: "Scheduler_Messaging",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Scheduler_Messaging",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DtAdd",
                table: "Scheduler_Messaging");

            migrationBuilder.DropColumn(
                name: "DtEnd",
                table: "Scheduler_Messaging");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Scheduler_Messaging");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DtStart",
                table: "Scheduler_Messaging",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}

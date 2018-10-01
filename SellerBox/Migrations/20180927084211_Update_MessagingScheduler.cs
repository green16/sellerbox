using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Update_MessagingScheduler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Scheduler_Messaging",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RecipientsCount",
                table: "Scheduler_Messaging",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "VkUserIds",
                table: "Scheduler_Messaging",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Scheduler_Messaging");

            migrationBuilder.DropColumn(
                name: "RecipientsCount",
                table: "Scheduler_Messaging");

            migrationBuilder.DropColumn(
                name: "VkUserIds",
                table: "Scheduler_Messaging");
        }
    }
}

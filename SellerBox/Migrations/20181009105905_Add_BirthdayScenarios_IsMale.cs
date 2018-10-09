using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Add_BirthdayScenarios_IsMale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMale",
                table: "BirthdayScenarios",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMale",
                table: "BirthdayScenarios");
        }
    }
}

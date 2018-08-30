using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class AddRanamePhoto_VkUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "VkUsers",
                newName: "PhotoSquare50");

            migrationBuilder.AddColumn<string>(
                name: "PhotoOrig400",
                table: "VkUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoOrig400",
                table: "VkUsers");

            migrationBuilder.RenameColumn(
                name: "PhotoSquare50",
                table: "VkUsers",
                newName: "Photo");
        }
    }
}

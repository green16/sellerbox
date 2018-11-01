using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Add_ShortUrls_IsSingleClick : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSingleClick",
                table: "ShortUrls",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSingleClick",
                table: "ShortUrls");
        }
    }
}

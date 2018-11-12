using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class AllowShortUrlsWithoutSubscribers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscriberRequired",
                table: "ShortUrls",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdSubscriber",
                table: "History_ShortUrlClicks",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscriberRequired",
                table: "ShortUrls");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdSubscriber",
                table: "History_ShortUrlClicks",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}

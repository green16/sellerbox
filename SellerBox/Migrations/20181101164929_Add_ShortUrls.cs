using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Add_ShortUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "VkCallbackMessages",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "ShortUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RedirectTo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortUrls_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History_ShortUrlClicks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdShortUrl = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_ShortUrlClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_ShortUrlClicks_ShortUrls_IdShortUrl",
                        column: x => x.IdShortUrl,
                        principalTable: "ShortUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_ShortUrlClicks_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_ShortUrlClicks_IdShortUrl",
                table: "History_ShortUrlClicks",
                column: "IdShortUrl");

            migrationBuilder.CreateIndex(
                name: "IX_History_ShortUrlClicks_IdSubscriber",
                table: "History_ShortUrlClicks",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrls_IdGroup",
                table: "ShortUrls",
                column: "IdGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History_ShortUrlClicks");

            migrationBuilder.DropTable(
                name: "ShortUrls");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "VkCallbackMessages");
        }
    }
}

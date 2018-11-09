using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Add_ShortUrlsPassedClicks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortUrlsPassedClicks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdShortUrlsScenario = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrlsPassedClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortUrlsPassedClicks_ShortUrlsScenarios_IdShortUrlsScenario",
                        column: x => x.IdShortUrlsScenario,
                        principalTable: "ShortUrlsScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShortUrlsPassedClicks_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsPassedClicks_IdShortUrlsScenario",
                table: "ShortUrlsPassedClicks",
                column: "IdShortUrlsScenario");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsPassedClicks_IdSubscriber",
                table: "ShortUrlsPassedClicks",
                column: "IdSubscriber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortUrlsPassedClicks");
        }
    }
}

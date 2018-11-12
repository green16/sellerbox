using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class Add_ShortUlsScenarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortUrlsScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    CheckAfterSeconds = table.Column<int>(nullable: false),
                    IdShortUrl = table.Column<Guid>(nullable: true),
                    CheckIsSubscriber = table.Column<bool>(nullable: false),
                    IdCheckingChainContent = table.Column<Guid>(nullable: false),
                    IdGoToChain = table.Column<Guid>(nullable: true),
                    IdGoToErrorChain1 = table.Column<Guid>(nullable: true),
                    IdGoToErrorChain2 = table.Column<Guid>(nullable: true),
                    IdGoToErrorChain3 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrlsScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortUrlsScenarios_ChainContents_IdCheckingChainContent",
                        column: x => x.IdCheckingChainContent,
                        principalTable: "ChainContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShortUrlsScenarios_Chains_IdGoToChain",
                        column: x => x.IdGoToChain,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShortUrlsScenarios_Chains_IdGoToErrorChain1",
                        column: x => x.IdGoToErrorChain1,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShortUrlsScenarios_Chains_IdGoToErrorChain2",
                        column: x => x.IdGoToErrorChain2,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShortUrlsScenarios_Chains_IdGoToErrorChain3",
                        column: x => x.IdGoToErrorChain3,
                        principalTable: "Chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShortUrlsScenarios_ShortUrls_IdShortUrl",
                        column: x => x.IdShortUrl,
                        principalTable: "ShortUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsScenarios_IdCheckingChainContent",
                table: "ShortUrlsScenarios",
                column: "IdCheckingChainContent");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsScenarios_IdGoToChain",
                table: "ShortUrlsScenarios",
                column: "IdGoToChain");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsScenarios_IdGoToErrorChain1",
                table: "ShortUrlsScenarios",
                column: "IdGoToErrorChain1");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsScenarios_IdGoToErrorChain2",
                table: "ShortUrlsScenarios",
                column: "IdGoToErrorChain2");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsScenarios_IdGoToErrorChain3",
                table: "ShortUrlsScenarios",
                column: "IdGoToErrorChain3");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlsScenarios_IdShortUrl",
                table: "ShortUrlsScenarios",
                column: "IdShortUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortUrlsScenarios");
        }
    }
}

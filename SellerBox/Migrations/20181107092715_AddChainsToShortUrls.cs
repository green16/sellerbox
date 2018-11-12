using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class AddChainsToShortUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdChain",
                table: "ShortUrls",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrls_IdChain",
                table: "ShortUrls",
                column: "IdChain");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortUrls_Chains_IdChain",
                table: "ShortUrls",
                column: "IdChain",
                principalTable: "Chains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortUrls_Chains_IdChain",
                table: "ShortUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortUrls_IdChain",
                table: "ShortUrls");

            migrationBuilder.DropColumn(
                name: "IdChain",
                table: "ShortUrls");
        }
    }
}

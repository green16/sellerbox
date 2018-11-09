using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class RepostScenario_AddErrorFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdGoToErrorChain2",
                table: "RepostScenarios",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdGoToErrorChain3",
                table: "RepostScenarios",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepostScenarios_IdGoToErrorChain2",
                table: "RepostScenarios",
                column: "IdGoToErrorChain2");

            migrationBuilder.CreateIndex(
                name: "IX_RepostScenarios_IdGoToErrorChain3",
                table: "RepostScenarios",
                column: "IdGoToErrorChain3");

            migrationBuilder.AddForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToErrorChain2",
                table: "RepostScenarios",
                column: "IdGoToErrorChain2",
                principalTable: "Chains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToErrorChain3",
                table: "RepostScenarios",
                column: "IdGoToErrorChain3",
                principalTable: "Chains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToErrorChain2",
                table: "RepostScenarios");

            migrationBuilder.DropForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToErrorChain3",
                table: "RepostScenarios");

            migrationBuilder.DropIndex(
                name: "IX_RepostScenarios_IdGoToErrorChain2",
                table: "RepostScenarios");

            migrationBuilder.DropIndex(
                name: "IX_RepostScenarios_IdGoToErrorChain3",
                table: "RepostScenarios");

            migrationBuilder.DropColumn(
                name: "IdGoToErrorChain2",
                table: "RepostScenarios");

            migrationBuilder.DropColumn(
                name: "IdGoToErrorChain3",
                table: "RepostScenarios");
        }
    }
}

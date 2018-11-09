using Microsoft.EntityFrameworkCore.Migrations;

namespace SellerBox.Migrations
{
    public partial class RepostScenario_ReanameField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToChain2",
                table: "RepostScenarios");

            migrationBuilder.RenameColumn(
                name: "IdGoToChain2",
                table: "RepostScenarios",
                newName: "IdGoToErrorChain1");

            migrationBuilder.RenameIndex(
                name: "IX_RepostScenarios_IdGoToChain2",
                table: "RepostScenarios",
                newName: "IX_RepostScenarios_IdGoToErrorChain1");

            migrationBuilder.AddForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToErrorChain1",
                table: "RepostScenarios",
                column: "IdGoToErrorChain1",
                principalTable: "Chains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToErrorChain1",
                table: "RepostScenarios");

            migrationBuilder.RenameColumn(
                name: "IdGoToErrorChain1",
                table: "RepostScenarios",
                newName: "IdGoToChain2");

            migrationBuilder.RenameIndex(
                name: "IX_RepostScenarios_IdGoToErrorChain1",
                table: "RepostScenarios",
                newName: "IX_RepostScenarios_IdGoToChain2");

            migrationBuilder.AddForeignKey(
                name: "FK_RepostScenarios_Chains_IdGoToChain2",
                table: "RepostScenarios",
                column: "IdGoToChain2",
                principalTable: "Chains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class Add_CheckedSubscribersInRepostScenarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckedSubscribersInRepostScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdRepostScenario = table.Column<Guid>(nullable: true),
                    DtCheck = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckedSubscribersInRepostScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckedSubscribersInRepostScenarios_RepostScenarios_IdRepostScenario",
                        column: x => x.IdRepostScenario,
                        principalTable: "RepostScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckedSubscribersInRepostScenarios_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckedSubscribersInRepostScenarios_IdRepostScenario",
                table: "CheckedSubscribersInRepostScenarios",
                column: "IdRepostScenario");

            migrationBuilder.CreateIndex(
                name: "IX_CheckedSubscribersInRepostScenarios_IdSubscriber",
                table: "CheckedSubscribersInRepostScenarios",
                column: "IdSubscriber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckedSubscribersInRepostScenarios");
        }
    }
}

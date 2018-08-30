using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class AddSegments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Segments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DtCreate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGroup = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Segments_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscribersInSegments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdSubscriber = table.Column<Guid>(nullable: false),
                    IdSegment = table.Column<Guid>(nullable: false),
                    DtAdd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribersInSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribersInSegments_Segments_IdSegment",
                        column: x => x.IdSegment,
                        principalTable: "Segments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscribersInSegments_Subscribers_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Segments_IdGroup",
                table: "Segments",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInSegments_IdSegment",
                table: "SubscribersInSegments",
                column: "IdSegment");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersInSegments_IdSubscriber",
                table: "SubscribersInSegments",
                column: "IdSubscriber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscribersInSegments");

            migrationBuilder.DropTable(
                name: "Segments");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class Add_GroupAdmins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_IdUser",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_IdUser",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Groups");

            migrationBuilder.CreateTable(
                name: "GroupAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<int>(nullable: false),
                    IdUser = table.Column<string>(nullable: true),
                    DtConnect = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAdmins_Groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "Groups",
                        principalColumn: "IdVk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAdmins_AspNetUsers_IdUser",
                        column: x => x.IdUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdmins_IdGroup",
                table: "GroupAdmins",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdmins_IdUser",
                table: "GroupAdmins",
                column: "IdUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAdmins");

            migrationBuilder.AddColumn<string>(
                name: "IdUser",
                table: "Groups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_IdUser",
                table: "Groups",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_IdUser",
                table: "Groups",
                column: "IdUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

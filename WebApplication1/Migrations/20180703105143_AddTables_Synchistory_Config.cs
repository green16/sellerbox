using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class AddTables_Synchistory_Config : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SyncHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdGroup = table.Column<int>(nullable: false),
                    SyncType = table.Column<byte>(nullable: false),
                    DtStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "SyncHistory");
        }
    }
}

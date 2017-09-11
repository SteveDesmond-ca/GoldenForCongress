using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GoldenForCongress.Migrations
{
    public partial class RouteSections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.CreateTable(
                name: "RouteSections",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteSections", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteSections");

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Color = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.ID);
                });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace TreeViewExample.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrgUnits",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentUnitId = table.Column<int>(type: "int", nullable: true),
                    OrgUnitType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnits", x => x.UnitId);
                    //table.ForeignKey(
                    //    name: "FK_OrgUnits_OrgUnits_ParentUnitId",
                    //    column: x => x.ParentUnitId,
                    //    principalTable: "OrgUnits",
                    //    principalColumn: "UnitId",
                    //    onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrgUnitTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnitTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "OrgUnitTypes",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "1", "cmp", "Company" });

            migrationBuilder.InsertData(
                table: "OrgUnitTypes",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "2", "sub", "SubUnit" });

            migrationBuilder.InsertData(
                table: "OrgUnitTypes",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "3", "ret", "RetailStore" });

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnits_ParentUnitId",
                table: "OrgUnits",
                column: "ParentUnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrgUnits");

            migrationBuilder.DropTable(
                name: "OrgUnitTypes");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.ProjectSearch.Service.Api.Migrations
{
    public partial class ColumnNameCorrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReauestInstant",
                table: "Requests",
                newName: "RequestInstant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestInstant",
                table: "Requests",
                newName: "ReauestInstant");
        }
    }
}

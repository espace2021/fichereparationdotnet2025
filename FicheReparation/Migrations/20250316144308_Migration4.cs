using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FicheReparation.Migrations
{
    public partial class Migration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "DemandeReparations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "DemandeReparations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

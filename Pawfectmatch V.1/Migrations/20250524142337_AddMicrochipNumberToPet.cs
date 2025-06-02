using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawfectmatch_V._1.Migrations
{
    /// <inheritdoc />
    public partial class AddMicrochipNumberToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MicrochipNumber",
                table: "Pets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MicrochipNumber",
                table: "Pets");
        }
    }
}

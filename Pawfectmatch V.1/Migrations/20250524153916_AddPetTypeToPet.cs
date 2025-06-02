using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawfectmatch_V._1.Migrations
{
    /// <inheritdoc />
    public partial class AddPetTypeToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PetType",
                table: "Pets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetType",
                table: "Pets");
        }
    }
}

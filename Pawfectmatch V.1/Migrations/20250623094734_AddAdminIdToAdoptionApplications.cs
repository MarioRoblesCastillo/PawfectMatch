using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawfectmatch_V._1.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminIdToAdoptionApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawfectmatch_V._1.Migrations
{
    /// <inheritdoc />
    public partial class LinkPetToAdoptionApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Breed",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "PetName",
                table: "AdoptionApplications");

            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "AdoptionApplications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionApplications_AdminId",
                table: "AdoptionApplications",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionApplications_PetId",
                table: "AdoptionApplications",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplications_AspNetUsers_AdminId",
                table: "AdoptionApplications",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplications_Pets_PetId",
                table: "AdoptionApplications",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplications_AspNetUsers_AdminId",
                table: "AdoptionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplications_Pets_PetId",
                table: "AdoptionApplications");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionApplications_AdminId",
                table: "AdoptionApplications");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionApplications_PetId",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "AdoptionApplications");

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PetName",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

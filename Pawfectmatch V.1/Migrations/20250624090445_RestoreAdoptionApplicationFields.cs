using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawfectmatch_V._1.Migrations
{
    /// <inheritdoc />
    public partial class RestoreAdoptionApplicationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowHomeVisit",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentlyHavePets",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinanciallyPrepared",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LandlordAllowsPets",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LongTermCommitment",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnOrRent",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnedPetsBefore",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetCareWhenAway",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResidenceType",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecureYard",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdoptionStories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StoryText = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    PhotoPath = table.Column<string>(type: "TEXT", nullable: false),
                    DatePosted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionStories_AdoptionApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "AdoptionApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionStories_ApplicationId",
                table: "AdoptionStories",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionStories");

            migrationBuilder.DropColumn(
                name: "AllowHomeVisit",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "CurrentlyHavePets",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "FinanciallyPrepared",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "LandlordAllowsPets",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "LongTermCommitment",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "OwnOrRent",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "OwnedPetsBefore",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "PetCareWhenAway",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "ResidenceType",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "SecureYard",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AdoptionApplications");
        }
    }
}

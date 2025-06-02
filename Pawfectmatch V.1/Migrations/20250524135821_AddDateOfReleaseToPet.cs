using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawfectmatch_V._1.Migrations
{
    /// <inheritdoc />
    public partial class AddDateOfReleaseToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfRelease",
                table: "Pets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfRelease",
                table: "Pets");
        }
    }
}

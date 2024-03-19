using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessUnitApp.Migrations
{
    /// <inheritdoc />
    public partial class addcolumnuploadby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadBy",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Documents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Documents");
        }
    }
}

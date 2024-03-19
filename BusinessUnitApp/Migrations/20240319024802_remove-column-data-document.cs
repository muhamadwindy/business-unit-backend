using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessUnitApp.Migrations
{
    /// <inheritdoc />
    public partial class removecolumndatadocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Documents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "Documents",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}

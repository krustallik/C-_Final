using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListenerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class screenshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Screenshots");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Screenshots",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Screenshots");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Screenshots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

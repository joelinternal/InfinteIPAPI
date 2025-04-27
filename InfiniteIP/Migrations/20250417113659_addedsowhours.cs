using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfiniteIP.Migrations
{
    /// <inheritdoc />
    public partial class addedsowhours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "hours",
                table: "GmSheet",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "sow",
                table: "GmSheet",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hours",
                table: "GmSheet");

            migrationBuilder.DropColumn(
                name: "sow",
                table: "GmSheet");
        }
    }
}

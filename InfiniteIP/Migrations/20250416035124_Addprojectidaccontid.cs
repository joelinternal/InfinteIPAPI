using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfiniteIP.Migrations
{
    /// <inheritdoc />
    public partial class Addprojectidaccontid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "brspdMgr",
                table: "GmSheet",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "accountId",
                table: "GmSheet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "projectId",
                table: "GmSheet",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accountId",
                table: "GmSheet");

            migrationBuilder.DropColumn(
                name: "projectId",
                table: "GmSheet");

            migrationBuilder.AlterColumn<string>(
                name: "brspdMgr",
                table: "GmSheet",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}

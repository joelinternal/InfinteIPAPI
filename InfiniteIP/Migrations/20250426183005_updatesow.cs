using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfiniteIP.Migrations
{
    /// <inheritdoc />
    public partial class updatesow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sow_Account_AccountId",
                table: "Sow");

            migrationBuilder.DropForeignKey(
                name: "FK_Sow_Projects_ProjectId",
                table: "Sow");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Sow",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Sow",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Sow_Account_AccountId",
                table: "Sow",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sow_Projects_ProjectId",
                table: "Sow",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sow_Account_AccountId",
                table: "Sow");

            migrationBuilder.DropForeignKey(
                name: "FK_Sow_Projects_ProjectId",
                table: "Sow");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Sow",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Sow",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sow_Account_AccountId",
                table: "Sow",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sow_Projects_ProjectId",
                table: "Sow",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

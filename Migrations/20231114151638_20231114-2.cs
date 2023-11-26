using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taxi_api.Migrations
{
    /// <inheritdoc />
    public partial class _202311142 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Drivers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_IdentityUserId",
                table: "Drivers",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_AspNetUsers_IdentityUserId",
                table: "Drivers",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_AspNetUsers_IdentityUserId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_IdentityUserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Drivers");
        }
    }
}

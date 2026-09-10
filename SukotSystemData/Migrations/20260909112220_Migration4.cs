using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SukotSystemData.Migrations
{
    /// <inheritdoc />
    public partial class Migration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "100000.bv0SLUsDGpMM+srg54YgjA==.SqnUa/+8WvKmTTcyx52+m68DRrEcoasVIUVsNyAcW6o=");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "Phone" },
                values: new object[] { "100000.bv0SLUsDGpMM+srg54YgjA==.SqnUa/+8WvKmTTcyx52+m68DRrEcoasVIUVsNyAcW6o=", "0500000000" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "1234");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "Phone" },
                values: new object[] { "1234", "0556751030" });
        }
    }
}

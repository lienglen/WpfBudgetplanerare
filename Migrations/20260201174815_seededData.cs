using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfBudgetplanerare.Migrations
{
    /// <inheritdoc />
    public partial class seededData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6,
                column: "Name",
                value: "Bonus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6,
                column: "Name",
                value: "Salery");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWEB_Proiect.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriorityFromPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Preference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Preference",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

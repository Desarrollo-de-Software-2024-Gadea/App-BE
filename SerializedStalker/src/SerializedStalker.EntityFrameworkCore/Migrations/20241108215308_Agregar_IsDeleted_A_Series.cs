using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerializedStalker.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_IsDeleted_A_Series : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppSeries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppSeries");
        }
    }
}

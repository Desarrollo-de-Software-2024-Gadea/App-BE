using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerializedStalker.Migrations
{
    /// <inheritdoc />
    public partial class addListaDeSeguimientoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListaDeSeguimientoId",
                table: "AppSeries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppListasDeSeguimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaModificacion = table.Column<DateOnly>(type: "date", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListasDeSeguimiento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSeries_ListaDeSeguimientoId",
                table: "AppSeries",
                column: "ListaDeSeguimientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSeries_AppListasDeSeguimiento_ListaDeSeguimientoId",
                table: "AppSeries",
                column: "ListaDeSeguimientoId",
                principalTable: "AppListasDeSeguimiento",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSeries_AppListasDeSeguimiento_ListaDeSeguimientoId",
                table: "AppSeries");

            migrationBuilder.DropTable(
                name: "AppListasDeSeguimiento");

            migrationBuilder.DropIndex(
                name: "IX_AppSeries_ListaDeSeguimientoId",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "ListaDeSeguimientoId",
                table: "AppSeries");
        }
    }
}

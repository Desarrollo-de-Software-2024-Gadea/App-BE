using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerializedStalker.Migrations
{
    /// <inheritdoc />
    public partial class addEpisodioEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImdbID",
                table: "AppSeries",
                newName: "ImdbIdentificator");

            migrationBuilder.CreateTable(
                name: "AppEpisodios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroEpisodio = table.Column<int>(type: "int", nullable: false),
                    FechaEstreno = table.Column<DateOnly>(type: "date", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Directores = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Escritores = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Duracion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TemporadaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEpisodios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEpisodios_AppTemporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "AppTemporadas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEpisodios_TemporadaId",
                table: "AppEpisodios",
                column: "TemporadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEpisodios");

            migrationBuilder.RenameColumn(
                name: "ImdbIdentificator",
                table: "AppSeries",
                newName: "ImdbID");
        }
    }
}

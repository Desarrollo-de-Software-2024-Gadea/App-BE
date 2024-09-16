using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerializedStalker.Migrations
{
    /// <inheritdoc />
    public partial class addEpisodioentity34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEpisodios_AppTemporadas_TemporadaId",
                table: "AppEpisodios");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTemporadas_AppSeries_SerieId",
                table: "AppTemporadas");

            migrationBuilder.RenameColumn(
                name: "SerieId",
                table: "AppTemporadas",
                newName: "SerieID");

            migrationBuilder.RenameIndex(
                name: "IX_AppTemporadas_SerieId",
                table: "AppTemporadas",
                newName: "IX_AppTemporadas_SerieID");

            migrationBuilder.RenameColumn(
                name: "TemporadaId",
                table: "AppEpisodios",
                newName: "TemporadaID");

            migrationBuilder.RenameIndex(
                name: "IX_AppEpisodios_TemporadaId",
                table: "AppEpisodios",
                newName: "IX_AppEpisodios_TemporadaID");

            migrationBuilder.AlterColumn<int>(
                name: "SerieID",
                table: "AppTemporadas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TemporadaID",
                table: "AppEpisodios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppEpisodios_AppTemporadas_TemporadaID",
                table: "AppEpisodios",
                column: "TemporadaID",
                principalTable: "AppTemporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTemporadas_AppSeries_SerieID",
                table: "AppTemporadas",
                column: "SerieID",
                principalTable: "AppSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEpisodios_AppTemporadas_TemporadaID",
                table: "AppEpisodios");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTemporadas_AppSeries_SerieID",
                table: "AppTemporadas");

            migrationBuilder.RenameColumn(
                name: "SerieID",
                table: "AppTemporadas",
                newName: "SerieId");

            migrationBuilder.RenameIndex(
                name: "IX_AppTemporadas_SerieID",
                table: "AppTemporadas",
                newName: "IX_AppTemporadas_SerieId");

            migrationBuilder.RenameColumn(
                name: "TemporadaID",
                table: "AppEpisodios",
                newName: "TemporadaId");

            migrationBuilder.RenameIndex(
                name: "IX_AppEpisodios_TemporadaID",
                table: "AppEpisodios",
                newName: "IX_AppEpisodios_TemporadaId");

            migrationBuilder.AlterColumn<int>(
                name: "SerieId",
                table: "AppTemporadas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TemporadaId",
                table: "AppEpisodios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEpisodios_AppTemporadas_TemporadaId",
                table: "AppEpisodios",
                column: "TemporadaId",
                principalTable: "AppTemporadas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTemporadas_AppSeries_SerieId",
                table: "AppTemporadas",
                column: "SerieId",
                principalTable: "AppSeries",
                principalColumn: "Id");
        }
    }
}

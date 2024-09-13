using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerializedStalker.Migrations
{
    /// <inheritdoc />
    public partial class addserieentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Clasificacion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FechaEstreno = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Duracion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Generos = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Directores = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Escritores = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Actores = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Sinopsis = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Idiomas = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Poster = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ImdbPuntuacion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ImdbVotos = table.Column<int>(type: "int", nullable: false),
                    ImdbID = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TotalTemporadas = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSeries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSeries");
        }
    }
}

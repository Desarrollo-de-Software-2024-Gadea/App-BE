using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerializedStalker.Migrations
{
    /// <inheritdoc />
    public partial class Add_User_To_Lista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                table: "AppListasDeSeguimiento",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppListasDeSeguimiento",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Creator",
                table: "AppListasDeSeguimiento");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppListasDeSeguimiento");
        }
    }
}

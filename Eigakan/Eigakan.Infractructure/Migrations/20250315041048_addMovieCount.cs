using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addMovieCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "MovieCount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: true),
                    MovieId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieCount_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieCount_MovieId",
                table: "MovieCount",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieCount");

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Movies",
                type: "int",
                nullable: true);
        }
    }
}

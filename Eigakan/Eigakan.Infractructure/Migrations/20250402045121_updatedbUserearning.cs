using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatedbUserearning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserEarning",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: true),
                    MonthYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalView = table.Column<int>(type: "int", nullable: true),
                    TotalEarning = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WebEarning = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovieId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEarning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEarning_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEarning_MovieId",
                table: "UserEarning",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEarning");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class contract1nmovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_MovieId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Terms",
                table: "Contracts");

            migrationBuilder.AddColumn<string>(
                name: "ExtendRequest",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtendStatus",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalContractId",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_MovieId",
                table: "Contracts",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_MovieId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ExtendRequest",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ExtendStatus",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "OriginalContractId",
                table: "Contracts");

            migrationBuilder.AddColumn<string>(
                name: "Terms",
                table: "Contracts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_MovieId",
                table: "Contracts",
                column: "MovieId",
                unique: true,
                filter: "[MovieId] IS NOT NULL");
        }
    }
}

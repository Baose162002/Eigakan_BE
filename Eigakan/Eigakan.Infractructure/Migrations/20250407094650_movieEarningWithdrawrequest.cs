using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class movieEarningWithdrawrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEarnings_Movies_MovieId",
                table: "UserEarnings");

            migrationBuilder.DropIndex(
                name: "IX_UserEarnings_MovieId",
                table: "UserEarnings");

            migrationBuilder.DropColumn(
                name: "MonthYear",
                table: "UserEarnings");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "UserEarnings");

            migrationBuilder.DropColumn(
                name: "WeekNumber",
                table: "UserEarnings");

            migrationBuilder.RenameColumn(
                name: "WebEarning",
                table: "UserEarnings",
                newName: "WebEarnings");

            migrationBuilder.RenameColumn(
                name: "TotalEarning",
                table: "UserEarnings",
                newName: "TotalEarnings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserEarnings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "UserEarnings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndWeek",
                table: "UserEarnings",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalEarnings",
                table: "UserEarnings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartWeek",
                table: "UserEarnings",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MovieEarnings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartWeek = table.Column<DateOnly>(type: "date", nullable: true),
                    EndWeek = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalView = table.Column<int>(type: "int", nullable: true),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovieId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieEarnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieEarnings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WithdrawRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequestAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserEarningId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawRequests_UserEarnings_UserEarningId",
                        column: x => x.UserEarningId,
                        principalTable: "UserEarnings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEarnings_UserId",
                table: "UserEarnings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieEarnings_MovieId",
                table: "MovieEarnings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawRequests_UserEarningId",
                table: "WithdrawRequests",
                column: "UserEarningId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEarnings_Users_UserId",
                table: "UserEarnings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEarnings_Users_UserId",
                table: "UserEarnings");

            migrationBuilder.DropTable(
                name: "MovieEarnings");

            migrationBuilder.DropTable(
                name: "WithdrawRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserEarnings_UserId",
                table: "UserEarnings");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "UserEarnings");

            migrationBuilder.DropColumn(
                name: "EndWeek",
                table: "UserEarnings");

            migrationBuilder.DropColumn(
                name: "FinalEarnings",
                table: "UserEarnings");

            migrationBuilder.DropColumn(
                name: "StartWeek",
                table: "UserEarnings");

            migrationBuilder.RenameColumn(
                name: "WebEarnings",
                table: "UserEarnings",
                newName: "WebEarning");

            migrationBuilder.RenameColumn(
                name: "TotalEarnings",
                table: "UserEarnings",
                newName: "TotalEarning");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserEarnings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonthYear",
                table: "UserEarnings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovieId",
                table: "UserEarnings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeekNumber",
                table: "UserEarnings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserEarnings_MovieId",
                table: "UserEarnings",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEarnings_Movies_MovieId",
                table: "UserEarnings",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");
        }
    }
}

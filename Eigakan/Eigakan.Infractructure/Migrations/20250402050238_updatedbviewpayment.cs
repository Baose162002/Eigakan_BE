using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatedbviewpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchases_RefundPolicy_RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCount_Movies_MovieId",
                table: "MovieCount");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieHistory_Movies_MovieId",
                table: "MovieHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieHistory_Users_UserId",
                table: "MovieHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEarning_Movies_MovieId",
                table: "UserEarning");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRegister_UserRegisterId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRegister",
                table: "UserRegister");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEarning",
                table: "UserEarning");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefundPolicy",
                table: "RefundPolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieHistory",
                table: "MovieHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCount",
                table: "MovieCount");

            migrationBuilder.RenameTable(
                name: "UserRegister",
                newName: "UserRegisters");

            migrationBuilder.RenameTable(
                name: "UserEarning",
                newName: "UserEarnings");

            migrationBuilder.RenameTable(
                name: "RefundPolicy",
                newName: "RefundPolicies");

            migrationBuilder.RenameTable(
                name: "MovieHistory",
                newName: "MovieHistories");

            migrationBuilder.RenameTable(
                name: "MovieCount",
                newName: "MovieCounts");

            migrationBuilder.RenameIndex(
                name: "IX_UserEarning_MovieId",
                table: "UserEarnings",
                newName: "IX_UserEarnings_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieHistory_UserId",
                table: "MovieHistories",
                newName: "IX_MovieHistories_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieHistory_MovieId",
                table: "MovieHistories",
                newName: "IX_MovieHistories_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCount_MovieId",
                table: "MovieCounts",
                newName: "IX_MovieCounts_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRegisters",
                table: "UserRegisters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEarnings",
                table: "UserEarnings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefundPolicies",
                table: "RefundPolicies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieHistories",
                table: "MovieHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCounts",
                table: "MovieCounts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AdMediaCounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: true),
                    MovieId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdMediaId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdMediaCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdMediaCounts_AdMedia_AdMediaId",
                        column: x => x.AdMediaId,
                        principalTable: "AdMedia",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ViewPaymentPolicies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PricePerView = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WebSharePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewPaymentPolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdMediaCounts_AdMediaId",
                table: "AdMediaCounts",
                column: "AdMediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchases_RefundPolicies_RefundPolicyID",
                table: "AdPurchases",
                column: "RefundPolicyID",
                principalTable: "RefundPolicies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCounts_Movies_MovieId",
                table: "MovieCounts",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieHistories_Movies_MovieId",
                table: "MovieHistories",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieHistories_Users_UserId",
                table: "MovieHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEarnings_Movies_MovieId",
                table: "UserEarnings",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRegisters_UserRegisterId",
                table: "Users",
                column: "UserRegisterId",
                principalTable: "UserRegisters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchases_RefundPolicies_RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCounts_Movies_MovieId",
                table: "MovieCounts");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieHistories_Movies_MovieId",
                table: "MovieHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieHistories_Users_UserId",
                table: "MovieHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEarnings_Movies_MovieId",
                table: "UserEarnings");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRegisters_UserRegisterId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AdMediaCounts");

            migrationBuilder.DropTable(
                name: "ViewPaymentPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRegisters",
                table: "UserRegisters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEarnings",
                table: "UserEarnings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefundPolicies",
                table: "RefundPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieHistories",
                table: "MovieHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCounts",
                table: "MovieCounts");

            migrationBuilder.RenameTable(
                name: "UserRegisters",
                newName: "UserRegister");

            migrationBuilder.RenameTable(
                name: "UserEarnings",
                newName: "UserEarning");

            migrationBuilder.RenameTable(
                name: "RefundPolicies",
                newName: "RefundPolicy");

            migrationBuilder.RenameTable(
                name: "MovieHistories",
                newName: "MovieHistory");

            migrationBuilder.RenameTable(
                name: "MovieCounts",
                newName: "MovieCount");

            migrationBuilder.RenameIndex(
                name: "IX_UserEarnings_MovieId",
                table: "UserEarning",
                newName: "IX_UserEarning_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieHistories_UserId",
                table: "MovieHistory",
                newName: "IX_MovieHistory_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieHistories_MovieId",
                table: "MovieHistory",
                newName: "IX_MovieHistory_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCounts_MovieId",
                table: "MovieCount",
                newName: "IX_MovieCount_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRegister",
                table: "UserRegister",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEarning",
                table: "UserEarning",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefundPolicy",
                table: "RefundPolicy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieHistory",
                table: "MovieHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCount",
                table: "MovieCount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchases_RefundPolicy_RefundPolicyID",
                table: "AdPurchases",
                column: "RefundPolicyID",
                principalTable: "RefundPolicy",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCount_Movies_MovieId",
                table: "MovieCount",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieHistory_Movies_MovieId",
                table: "MovieHistory",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieHistory_Users_UserId",
                table: "MovieHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEarning_Movies_MovieId",
                table: "UserEarning",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRegister_UserRegisterId",
                table: "Users",
                column: "UserRegisterId",
                principalTable: "UserRegister",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

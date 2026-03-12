using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatedbADS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdMedia_AdPurchaseSlots_AdPurchaseSlotId",
                table: "AdMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchases_RefundPolicies_RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchases_Users_UserId",
                table: "AdPurchases");

            migrationBuilder.DropTable(
                name: "AdPurchaseSlots");

            migrationBuilder.DropTable(
                name: "RefundPolicies");

            migrationBuilder.DropTable(
                name: "AdSlotTimes");

            migrationBuilder.DropTable(
                name: "AdSlotTimeRanges");

            migrationBuilder.DropTable(
                name: "AdSlots");

            migrationBuilder.DropIndex(
                name: "IX_AdMedia_AdPurchaseSlotId",
                table: "AdMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdPurchases",
                table: "AdPurchases");

            migrationBuilder.DropIndex(
                name: "IX_AdPurchases_RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "AdPackages");

            migrationBuilder.DropColumn(
                name: "PackPrice",
                table: "AdPackages");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "AdPackages");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "AdMediaCounts");

            migrationBuilder.DropColumn(
                name: "AdPurchaseSlotId",
                table: "AdMedia");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "AdMedia");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "AdMedia");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "PaymentReferenceID",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "RefundEvidence",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "RefundPrice",
                table: "AdPurchases");

            migrationBuilder.RenameTable(
                name: "AdPurchases",
                newName: "AdPurchasesTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_AdPurchases_UserId",
                table: "AdPurchasesTransaction",
                newName: "IX_AdPurchasesTransaction_UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "AdPackages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxView",
                table: "AdPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinView",
                table: "AdPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerView",
                table: "AdPackages",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdPurchasesTransaction",
                table: "AdPurchasesTransaction",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AdPurchaseItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewQuantity = table.Column<int>(type: "int", nullable: true),
                    PricePerView = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RemainingViews = table.Column<int>(type: "int", nullable: true),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdPackageId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdMediaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdPurchaseTransactionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdPurchaseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdPurchaseItems_AdMedia_AdMediaId",
                        column: x => x.AdMediaId,
                        principalTable: "AdMedia",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdPurchaseItems_AdPackages_AdPackageId",
                        column: x => x.AdPackageId,
                        principalTable: "AdPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdPurchaseItems_AdPurchasesTransaction_AdPurchaseTransactionId",
                        column: x => x.AdPurchaseTransactionId,
                        principalTable: "AdPurchasesTransaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserWallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMenthod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserWalletId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_UserWallets_UserWalletId",
                        column: x => x.UserWalletId,
                        principalTable: "UserWallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchaseItems_AdMediaId",
                table: "AdPurchaseItems",
                column: "AdMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchaseItems_AdPackageId",
                table: "AdPurchaseItems",
                column: "AdPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchaseItems_AdPurchaseTransactionId",
                table: "AdPurchaseItems",
                column: "AdPurchaseTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_UserId",
                table: "UserWallets",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_UserWalletId",
                table: "WalletTransactions",
                column: "UserWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchasesTransaction_Users_UserId",
                table: "AdPurchasesTransaction",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchasesTransaction_Users_UserId",
                table: "AdPurchasesTransaction");

            migrationBuilder.DropTable(
                name: "AdPurchaseItems");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "UserWallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdPurchasesTransaction",
                table: "AdPurchasesTransaction");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "AdPackages");

            migrationBuilder.DropColumn(
                name: "MaxView",
                table: "AdPackages");

            migrationBuilder.DropColumn(
                name: "MinView",
                table: "AdPackages");

            migrationBuilder.DropColumn(
                name: "PricePerView",
                table: "AdPackages");

            migrationBuilder.RenameTable(
                name: "AdPurchasesTransaction",
                newName: "AdPurchases");

            migrationBuilder.RenameIndex(
                name: "IX_AdPurchasesTransaction_UserId",
                table: "AdPurchases",
                newName: "IX_AdPurchases_UserId");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "AdPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PackPrice",
                table: "AdPackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "AdPackages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MovieId",
                table: "AdMediaCounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdPurchaseSlotId",
                table: "AdMedia",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "AdMedia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "AdMedia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "AdPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "AdPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReferenceID",
                table: "AdPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundEvidence",
                table: "AdPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundPolicyID",
                table: "AdPurchases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundPrice",
                table: "AdPurchases",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdPurchases",
                table: "AdPurchases",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AdSlots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SlotLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SlotPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdSlotTimeRanges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    SlotTimeRangePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdSlotTimeRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefundPolicies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Max = table.Column<int>(type: "int", nullable: true),
                    Min = table.Column<int>(type: "int", nullable: true),
                    PolicyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefundPercent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdSlotTimes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdSlotID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdSlotTimeRangeID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: true),
                    SlotTimePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdSlotTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdSlotTimes_AdSlotTimeRanges_AdSlotTimeRangeID",
                        column: x => x.AdSlotTimeRangeID,
                        principalTable: "AdSlotTimeRanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdSlotTimes_AdSlots_AdSlotID",
                        column: x => x.AdSlotID,
                        principalTable: "AdSlots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AdPurchaseSlots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdPackageID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdPurchaseID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdSlotTimeID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchaseSlotPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdPurchaseSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdPurchaseSlots_AdPackages_AdPackageID",
                        column: x => x.AdPackageID,
                        principalTable: "AdPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdPurchaseSlots_AdPurchases_AdPurchaseID",
                        column: x => x.AdPurchaseID,
                        principalTable: "AdPurchases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdPurchaseSlots_AdSlotTimes_AdSlotTimeID",
                        column: x => x.AdSlotTimeID,
                        principalTable: "AdSlotTimes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdMedia_AdPurchaseSlotId",
                table: "AdMedia",
                column: "AdPurchaseSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchases_RefundPolicyID",
                table: "AdPurchases",
                column: "RefundPolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchaseSlots_AdPackageID",
                table: "AdPurchaseSlots",
                column: "AdPackageID");

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchaseSlots_AdPurchaseID",
                table: "AdPurchaseSlots",
                column: "AdPurchaseID");

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchaseSlots_AdSlotTimeID",
                table: "AdPurchaseSlots",
                column: "AdSlotTimeID");

            migrationBuilder.CreateIndex(
                name: "IX_AdSlotTimes_AdSlotID",
                table: "AdSlotTimes",
                column: "AdSlotID");

            migrationBuilder.CreateIndex(
                name: "IX_AdSlotTimes_AdSlotTimeRangeID",
                table: "AdSlotTimes",
                column: "AdSlotTimeRangeID");

            migrationBuilder.AddForeignKey(
                name: "FK_AdMedia_AdPurchaseSlots_AdPurchaseSlotId",
                table: "AdMedia",
                column: "AdPurchaseSlotId",
                principalTable: "AdPurchaseSlots",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchases_RefundPolicies_RefundPolicyID",
                table: "AdPurchases",
                column: "RefundPolicyID",
                principalTable: "RefundPolicies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchases_Users_UserId",
                table: "AdPurchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

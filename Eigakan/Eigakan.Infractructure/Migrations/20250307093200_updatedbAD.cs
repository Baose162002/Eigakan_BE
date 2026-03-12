using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatedbAD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchases_Users_UserID",
                table: "AdPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchaseSlots_AdPackages_AdPackageID",
                table: "AdPurchaseSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchaseSlots_AdPurchases_AdPurchaseID",
                table: "AdPurchaseSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchaseSlots_AdSlotTimes_AdSlotTimeID",
                table: "AdPurchaseSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AdSlotTimes_AdSlotTimeRanges_AdSlotTimeRangeID",
                table: "AdSlotTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_AdSlotTimes_AdSlots_AdSlotID",
                table: "AdSlotTimes");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "AdPurchaseSlots");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "AdPurchaseSlots");

            migrationBuilder.DropColumn(
                name: "PurchaseSlotPrice",
                table: "AdPurchaseSlots");

            migrationBuilder.DropColumn(
                name: "ReasonForRejection",
                table: "AdPurchaseSlots");

            migrationBuilder.DropColumn(
                name: "UrlLink",
                table: "AdPurchaseSlots");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "AdPurchaseSlots");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "AdPurchases");

            migrationBuilder.RenameColumn(
                name: "ApprovalDate",
                table: "AdPurchaseSlots",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "AdPurchases",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AdPurchases_UserID",
                table: "AdPurchases",
                newName: "IX_AdPurchases_UserId");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "SubscriptionPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReferenceID",
                table: "SubscriptionPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdSlotTimes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AdSlotTimeRangeID",
                table: "AdSlotTimes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "AdSlotID",
                table: "AdSlotTimes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdSlotTimeRanges",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "AdSlotTimeRanges",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "AdSlotTimeRanges",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateAt",
                table: "AdSlotTimeRanges",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdSlots",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SlotLocation",
                table: "AdSlots",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdPurchaseSlots",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AdSlotTimeID",
                table: "AdPurchaseSlots",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "AdPurchaseID",
                table: "AdPurchaseSlots",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "AdPackageID",
                table: "AdPurchaseSlots",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AdPurchases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "AdPurchases",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "AdPurchases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "AdPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReferenceID",
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

            migrationBuilder.AddColumn<string>(
                name: "RefundStatus",
                table: "AdPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AdPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdPackages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "AdMedia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Video = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForRejection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdPurchaseSlotId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdMedia_AdPurchaseSlots_AdPurchaseSlotId",
                        column: x => x.AdPurchaseSlotId,
                        principalTable: "AdPurchaseSlots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefundPolicy",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PolicyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefundPercent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundPolicy", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdPurchases_RefundPolicyID",
                table: "AdPurchases",
                column: "RefundPolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_AdMedia_AdPurchaseSlotId",
                table: "AdMedia",
                column: "AdPurchaseSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchases_RefundPolicy_RefundPolicyID",
                table: "AdPurchases",
                column: "RefundPolicyID",
                principalTable: "RefundPolicy",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchases_Users_UserId",
                table: "AdPurchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchaseSlots_AdPackages_AdPackageID",
                table: "AdPurchaseSlots",
                column: "AdPackageID",
                principalTable: "AdPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchaseSlots_AdPurchases_AdPurchaseID",
                table: "AdPurchaseSlots",
                column: "AdPurchaseID",
                principalTable: "AdPurchases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchaseSlots_AdSlotTimes_AdSlotTimeID",
                table: "AdPurchaseSlots",
                column: "AdSlotTimeID",
                principalTable: "AdSlotTimes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdSlotTimes_AdSlotTimeRanges_AdSlotTimeRangeID",
                table: "AdSlotTimes",
                column: "AdSlotTimeRangeID",
                principalTable: "AdSlotTimeRanges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdSlotTimes_AdSlots_AdSlotID",
                table: "AdSlotTimes",
                column: "AdSlotID",
                principalTable: "AdSlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchases_RefundPolicy_RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchases_Users_UserId",
                table: "AdPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchaseSlots_AdPackages_AdPackageID",
                table: "AdPurchaseSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchaseSlots_AdPurchases_AdPurchaseID",
                table: "AdPurchaseSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPurchaseSlots_AdSlotTimes_AdSlotTimeID",
                table: "AdPurchaseSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AdSlotTimes_AdSlotTimeRanges_AdSlotTimeRangeID",
                table: "AdSlotTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_AdSlotTimes_AdSlots_AdSlotID",
                table: "AdSlotTimes");

            migrationBuilder.DropTable(
                name: "AdMedia");

            migrationBuilder.DropTable(
                name: "RefundPolicy");

            migrationBuilder.DropIndex(
                name: "IX_AdPurchases_RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "SubscriptionPurchases");

            migrationBuilder.DropColumn(
                name: "PaymentReferenceID",
                table: "SubscriptionPurchases");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "PaymentReferenceID",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "RefundPolicyID",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "RefundPrice",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AdPurchases");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "AdPurchaseSlots",
                newName: "ApprovalDate");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AdPurchases",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_AdPurchases_UserId",
                table: "AdPurchases",
                newName: "IX_AdPurchases_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdSlotTimes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdSlotTimeRangeID",
                table: "AdSlotTimes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdSlotID",
                table: "AdSlotTimes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdSlotTimeRanges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "AdSlotTimeRanges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "AdSlotTimeRanges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateAt",
                table: "AdSlotTimeRanges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SlotLocation",
                table: "AdSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdPurchaseSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdSlotTimeID",
                table: "AdPurchaseSlots",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdPurchaseID",
                table: "AdPurchaseSlots",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdPackageID",
                table: "AdPurchaseSlots",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "AdPurchaseSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "AdPurchaseSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseSlotPrice",
                table: "AdPurchaseSlots",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForRejection",
                table: "AdPurchaseSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlLink",
                table: "AdPurchaseSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "AdPurchaseSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "AdPurchases",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "AdPurchases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "AdPurchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AdPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchases_Users_UserID",
                table: "AdPurchases",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchaseSlots_AdPackages_AdPackageID",
                table: "AdPurchaseSlots",
                column: "AdPackageID",
                principalTable: "AdPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchaseSlots_AdPurchases_AdPurchaseID",
                table: "AdPurchaseSlots",
                column: "AdPurchaseID",
                principalTable: "AdPurchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdPurchaseSlots_AdSlotTimes_AdSlotTimeID",
                table: "AdPurchaseSlots",
                column: "AdSlotTimeID",
                principalTable: "AdSlotTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdSlotTimes_AdSlotTimeRanges_AdSlotTimeRangeID",
                table: "AdSlotTimes",
                column: "AdSlotTimeRangeID",
                principalTable: "AdSlotTimeRanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdSlotTimes_AdSlots_AdSlotID",
                table: "AdSlotTimes",
                column: "AdSlotID",
                principalTable: "AdSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

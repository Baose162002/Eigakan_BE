using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class refundpolicyExtendcontract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefundStatus",
                table: "AdPurchases",
                newName: "RefundEvidence");

            migrationBuilder.AddColumn<int>(
                name: "Max",
                table: "RefundPolicies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Min",
                table: "RefundPolicies",
                type: "int",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Max",
                table: "RefundPolicies");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "RefundPolicies");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "AdPurchases");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "AdPurchases");

            migrationBuilder.RenameColumn(
                name: "RefundEvidence",
                table: "AdPurchases",
                newName: "RefundStatus");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eigakan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatedbViewPaymentPolicyStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ViewPaymentPolicies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ViewPaymentPolicies");
        }
    }
}

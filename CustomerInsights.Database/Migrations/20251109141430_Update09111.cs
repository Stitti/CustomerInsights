using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInsights.Database.Migrations
{
    /// <inheritdoc />
    public partial class Update09111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interactions_accounts_account_id",
                table: "interactions");

            migrationBuilder.AddForeignKey(
                name: "FK_interactions_accounts_account_id",
                table: "interactions",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interactions_accounts_account_id",
                table: "interactions");

            migrationBuilder.AddForeignKey(
                name: "FK_interactions_accounts_account_id",
                table: "interactions",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id");
        }
    }
}

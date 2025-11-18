using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInsights.Database.Migrations
{
    /// <inheritdoc />
    public partial class Update_081125 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_signals_account_id",
                table: "signals",
                column: "account_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_signals_accounts_account_id",
                table: "signals",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_signals_accounts_account_id",
                table: "signals");

            migrationBuilder.DropIndex(
                name: "IX_signals_account_id",
                table: "signals");
        }
    }
}

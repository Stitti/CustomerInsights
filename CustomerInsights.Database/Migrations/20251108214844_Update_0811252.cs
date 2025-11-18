using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInsights.Database.Migrations
{
    /// <inheritdoc />
    public partial class Update_0811252 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_signals_account_id",
                table: "signals");

            migrationBuilder.CreateIndex(
                name: "IX_signals_account_id",
                table: "signals",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_signals_account_id",
                table: "signals");

            migrationBuilder.CreateIndex(
                name: "IX_signals_account_id",
                table: "signals",
                column: "account_id",
                unique: true);
        }
    }
}

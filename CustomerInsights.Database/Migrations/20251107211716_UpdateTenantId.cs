using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInsights.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "external_id",
                table: "contacts",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "accounts",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "external_id",
                table: "accounts",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "external_id",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "country",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "external_id",
                table: "accounts");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInsights.Database.Migrations
{
    /// <inheritdoc />
    public partial class Update2111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "signals",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    last_chars = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    token_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    revoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    content = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    language_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    is_html = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_templates", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_keys");

            migrationBuilder.DropTable(
                name: "email_templates");

            migrationBuilder.DropColumn(
                name: "description",
                table: "signals");

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    processed_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.id);
                });
        }
    }
}

using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInsights.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    parent_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    industry = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    classification = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_accounts_accounts_parent_account_id",
                        column: x => x.parent_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "signals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    severity = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ttl_days = table.Column<int>(type: "integer", nullable: false),
                    dedupe_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    account_satisfaction_index = table.Column<double>(type: "double precision", nullable: false),
                    threshold = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signals", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    firstname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    lastname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.id);
                    table.ForeignKey(
                        name: "FK_contacts_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "satisfaction_state",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    satisfaction_index = table.Column<int>(type: "integer", nullable: false),
                    decayed_weighted_sum = table.Column<double>(type: "double precision", nullable: false),
                    decayed_weight_sum = table.Column<double>(type: "double precision", nullable: false),
                    config_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_satisfaction_state", x => new { x.tenant_id, x.account_id });
                    table.ForeignKey(
                        name: "FK_satisfaction_state_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "interactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    external_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    channel = table.Column<int>(type: "integer", nullable: false),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    analyzed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    contact_id = table.Column<Guid>(type: "uuid", nullable: true),
                    thread_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subject = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    meta = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_interactions_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_interactions_contacts_contact_id",
                        column: x => x.contact_id,
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "text_inference",
                columns: table => new
                {
                    interaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sentiment = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    sentiment_score = table.Column<double>(type: "double precision", nullable: false),
                    urgency = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    urgency_score = table.Column<double>(type: "double precision", nullable: false),
                    inferred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    extra = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_text_inference", x => x.interaction_id);
                    table.ForeignKey(
                        name: "FK_text_inference_interactions_interaction_id",
                        column: x => x.interaction_id,
                        principalTable: "interactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "text_inference_aspects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aspect_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: false),
                    text_inference_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_text_inference_aspects", x => x.id);
                    table.ForeignKey(
                        name: "FK_text_inference_aspects_text_inference_text_inference_id",
                        column: x => x.text_inference_id,
                        principalTable: "text_inference",
                        principalColumn: "interaction_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "text_inference_emotions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: false),
                    text_inference_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_text_inference_emotions", x => x.id);
                    table.ForeignKey(
                        name: "FK_text_inference_emotions_text_inference_text_inference_id",
                        column: x => x.text_inference_id,
                        principalTable: "text_inference",
                        principalColumn: "interaction_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_parent_account_id",
                table: "accounts",
                column: "parent_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_account_id",
                table: "contacts",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_interactions_account_id",
                table: "interactions",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_interactions_contact_id",
                table: "interactions",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_satisfaction_state_account_id",
                table: "satisfaction_state",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_signals_dedupe_key",
                table: "signals",
                column: "dedupe_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_signals_tenant_id_account_id",
                table: "signals",
                columns: new[] { "tenant_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "IX_text_inference_aspects_text_inference_id",
                table: "text_inference_aspects",
                column: "text_inference_id");

            migrationBuilder.CreateIndex(
                name: "IX_text_inference_emotions_text_inference_id",
                table: "text_inference_emotions",
                column: "text_inference_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "satisfaction_state");

            migrationBuilder.DropTable(
                name: "signals");

            migrationBuilder.DropTable(
                name: "text_inference_aspects");

            migrationBuilder.DropTable(
                name: "text_inference_emotions");

            migrationBuilder.DropTable(
                name: "text_inference");

            migrationBuilder.DropTable(
                name: "interactions");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}

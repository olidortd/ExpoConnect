using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpoConnect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:visit_sticker", "innovative,professional,friendly,informative,high_quality,good_value,impressive,would_recommend");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    display_name = table.Column<string>(type: "varchar", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "varchar", maxLength: 30, nullable: true),
                    company = table.Column<string>(type: "varchar", maxLength: 200, nullable: true),
                    role = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "bool", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    refresh_token_hash = table.Column<string>(type: "text", nullable: true),
                    refresh_token_expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "auth_refresh_token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    replaced_by_token = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_auth_refresh_token_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stand",
                columns: table => new
                {
                    qr_code = table.Column<string>(type: "varchar", maxLength: 128, nullable: false),
                    stand_number = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    company_name = table.Column<string>(type: "varchar", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    industry = table.Column<string>(type: "text", nullable: false),
                    contact_email = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    contact_phone = table.Column<string>(type: "varchar", maxLength: 30, nullable: true),
                    website = table.Column<string>(type: "text", nullable: true),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    banner_url = table.Column<string>(type: "text", nullable: true),
                    exhibitor_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stand", x => x.qr_code);
                    table.ForeignKey(
                        name: "fk_stand_users",
                        column: x => x.exhibitor_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_credentials",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_credentials", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_credentials_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catalogs",
                columns: table => new
                {
                    catalog_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stand_id = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalogs", x => x.catalog_id);
                    table.ForeignKey(
                        name: "fk_catalog_stand",
                        column: x => x.stand_id,
                        principalTable: "stand",
                        principalColumn: "qr_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visit",
                columns: table => new
                {
                    visit_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    visitor_id = table.Column<string>(type: "text", nullable: false),
                    stand_id = table.Column<string>(type: "varchar", maxLength: 128, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<int>(type: "int4", nullable: true),
                    stickers = table.Column<int[]>(type: "visit_sticker[]", nullable: true),
                    is_favorite = table.Column<bool>(type: "bool", nullable: false, defaultValue: false),
                    follow_up = table.Column<bool>(type: "bool", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_visit", x => x.visit_id);
                    table.ForeignKey(
                        name: "fk_visit_stand",
                        column: x => x.stand_id,
                        principalTable: "stand",
                        principalColumn: "qr_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_visit_users",
                        column: x => x.visitor_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catalog_item",
                columns: table => new
                {
                    item_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    catalog_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "varchar", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    features = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_item", x => x.item_id);
                    table.ForeignKey(
                        name: "fk_catalog_item_catalog",
                        column: x => x.catalog_id,
                        principalTable: "catalogs",
                        principalColumn: "catalog_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auth_refresh_token_token",
                table: "auth_refresh_token",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_auth_refresh_token_user_id",
                table: "auth_refresh_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_item_catalog_id",
                table: "catalog_item",
                column: "catalog_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_item_catalog_id_name",
                table: "catalog_item",
                columns: new[] { "catalog_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_catalog_item_category",
                table: "catalog_item",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_catalogs_stand_id",
                table: "catalogs",
                column: "stand_id");

            migrationBuilder.CreateIndex(
                name: "ix_stand_exhibitor_id",
                table: "stand",
                column: "exhibitor_id");

            migrationBuilder.CreateIndex(
                name: "ix_stand_stand_number",
                table: "stand",
                column: "stand_number");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_is_active",
                table: "users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_users_role",
                table: "users",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "ix_visit_rating",
                table: "visit",
                column: "rating");

            migrationBuilder.CreateIndex(
                name: "ix_visit_stand_id",
                table: "visit",
                column: "stand_id");

            migrationBuilder.CreateIndex(
                name: "ix_visit_visitor_id",
                table: "visit",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "ix_visit_visitor_id_stand_id",
                table: "visit",
                columns: new[] { "visitor_id", "stand_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth_refresh_token");

            migrationBuilder.DropTable(
                name: "catalog_item");

            migrationBuilder.DropTable(
                name: "user_credentials");

            migrationBuilder.DropTable(
                name: "visit");

            migrationBuilder.DropTable(
                name: "catalogs");

            migrationBuilder.DropTable(
                name: "stand");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryService.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddClientAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client_addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_addresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_client_addresses_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_client_addresses_client_id",
                table: "client_addresses",
                column: "client_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "client_addresses");
        }
    }
}

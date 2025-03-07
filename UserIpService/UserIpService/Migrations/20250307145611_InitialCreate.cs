using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserIpService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IpAddresses",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpAddresses", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserIps",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IpAddressId = table.Column<string>(type: "text", nullable: false),
                    ConnectionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIps", x => new { x.UserId, x.IpAddressId });
                    table.ForeignKey(
                        name: "FK_UserIps_IpAddresses_IpAddressId",
                        column: x => x.IpAddressId,
                        principalTable: "IpAddresses",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserIps_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserIps_IpAddressId",
                table: "UserIps",
                column: "IpAddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserIps");

            migrationBuilder.DropTable(
                name: "IpAddresses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

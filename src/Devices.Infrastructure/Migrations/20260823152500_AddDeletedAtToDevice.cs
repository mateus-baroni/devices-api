using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAtToDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "devices",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "devices");
        }
    }
}

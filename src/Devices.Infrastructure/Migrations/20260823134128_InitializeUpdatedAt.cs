using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitializeUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE devices
                SET "UpdatedAt" = "CreatedAt";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

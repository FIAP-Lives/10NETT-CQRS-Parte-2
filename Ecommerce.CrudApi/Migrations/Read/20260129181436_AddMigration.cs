using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.CrudApi.Migrations.Read
{
    /// <inheritdoc />
    public partial class AddMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "orders_read");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "orders_read",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "orders_read");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "orders_read",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

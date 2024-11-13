using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stato",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stato",
                table: "Ordini");
        }
    }
}

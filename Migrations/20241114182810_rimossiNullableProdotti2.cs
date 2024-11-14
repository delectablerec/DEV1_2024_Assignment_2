using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class rimossiNullableProdotti2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiametroId",
                table: "Prodotti");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiametroId",
                table: "Prodotti",
                type: "INTEGER",
                nullable: true);
        }
    }
}

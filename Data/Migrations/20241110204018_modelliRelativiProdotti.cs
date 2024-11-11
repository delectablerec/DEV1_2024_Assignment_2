using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchStoreApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class modelliRelativiProdotti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Generi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marche",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marche", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materiali",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiali", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tipologie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipologie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prodotti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prezzo = table.Column<decimal>(type: "TEXT", nullable: false),
                    Giacenza = table.Column<int>(type: "INTEGER", nullable: false),
                    Colore = table.Column<string>(type: "TEXT", nullable: true),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: true),
                    MarcaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Modello = table.Column<string>(type: "TEXT", nullable: true),
                    Referenza = table.Column<string>(type: "TEXT", nullable: true),
                    MaterialeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TipologiaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Diametro = table.Column<int>(type: "INTEGER", nullable: true),
                    GenereId = table.Column<int>(type: "INTEGER", nullable: true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prodotti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prodotti_Categorie_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorie",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prodotti_Generi_GenereId",
                        column: x => x.GenereId,
                        principalTable: "Generi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prodotti_Marche_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marche",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prodotti_Materiali_MaterialeId",
                        column: x => x.MaterialeId,
                        principalTable: "Materiali",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prodotti_Tipologie_TipologiaId",
                        column: x => x.TipologiaId,
                        principalTable: "Tipologie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prodotti_CategoriaId",
                table: "Prodotti",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Prodotti_GenereId",
                table: "Prodotti",
                column: "GenereId");

            migrationBuilder.CreateIndex(
                name: "IX_Prodotti_MarcaId",
                table: "Prodotti",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Prodotti_MaterialeId",
                table: "Prodotti",
                column: "MaterialeId");

            migrationBuilder.CreateIndex(
                name: "IX_Prodotti_TipologiaId",
                table: "Prodotti",
                column: "TipologiaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prodotti");

            migrationBuilder.DropTable(
                name: "Categorie");

            migrationBuilder.DropTable(
                name: "Generi");

            migrationBuilder.DropTable(
                name: "Marche");

            migrationBuilder.DropTable(
                name: "Materiali");

            migrationBuilder.DropTable(
                name: "Tipologie");
        }
    }
}

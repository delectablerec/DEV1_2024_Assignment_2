using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class creazioneProdotti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ordini_AspNetUsers_ClienteId",
                table: "Ordini");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordini_Prodotti_OrologioId",
                table: "Ordini");

            migrationBuilder.DropForeignKey(
                name: "FK_Prodotti_Categorie_CategoriaId",
                table: "Prodotti");

            migrationBuilder.DropForeignKey(
                name: "FK_Prodotti_Marche_MarcaId",
                table: "Prodotti");

            migrationBuilder.DropIndex(
                name: "IX_Ordini_ClienteId",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Tipologie");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Marche");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Generi");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Categorie");

            migrationBuilder.AlterColumn<int>(
                name: "MarcaId",
                table: "Prodotti",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Prodotti",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiametroId",
                table: "Prodotti",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlImmagine",
                table: "Prodotti",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "OrologioId",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ClienteId1",
                table: "Ordini",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "UrlImmagine",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ordini_ClienteId1",
                table: "Ordini",
                column: "ClienteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Ordini_AspNetUsers_ClienteId1",
                table: "Ordini",
                column: "ClienteId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordini_Prodotti_OrologioId",
                table: "Ordini",
                column: "OrologioId",
                principalTable: "Prodotti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prodotti_Categorie_CategoriaId",
                table: "Prodotti",
                column: "CategoriaId",
                principalTable: "Categorie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prodotti_Marche_MarcaId",
                table: "Prodotti",
                column: "MarcaId",
                principalTable: "Marche",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ordini_AspNetUsers_ClienteId1",
                table: "Ordini");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordini_Prodotti_OrologioId",
                table: "Ordini");

            migrationBuilder.DropForeignKey(
                name: "FK_Prodotti_Categorie_CategoriaId",
                table: "Prodotti");

            migrationBuilder.DropForeignKey(
                name: "FK_Prodotti_Marche_MarcaId",
                table: "Prodotti");

            migrationBuilder.DropIndex(
                name: "IX_Ordini_ClienteId1",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "DiametroId",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "UrlImmagine",
                table: "Prodotti");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "UrlImmagine",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Tipologie",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Tipologie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Tipologie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Tipologie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Tipologie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Tipologie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MarcaId",
                table: "Prodotti",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Prodotti",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Prodotti",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Prodotti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Prodotti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Prodotti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Prodotti",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Prodotti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrologioId",
                table: "Ordini",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ClienteId",
                table: "Ordini",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Ordini",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Ordini",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Materiali",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Materiali",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Materiali",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Materiali",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Materiali",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Marche",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Marche",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Marche",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Marche",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Marche",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Marche",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Generi",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Generi",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Generi",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Generi",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Generi",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Generi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Categorie",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Categorie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Categorie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Categorie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Categorie",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Categorie",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ordini_ClienteId",
                table: "Ordini",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ordini_AspNetUsers_ClienteId",
                table: "Ordini",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordini_Prodotti_OrologioId",
                table: "Ordini",
                column: "OrologioId",
                principalTable: "Prodotti",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prodotti_Categorie_CategoriaId",
                table: "Prodotti",
                column: "CategoriaId",
                principalTable: "Categorie",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prodotti_Marche_MarcaId",
                table: "Prodotti",
                column: "MarcaId",
                principalTable: "Marche",
                principalColumn: "Id");
        }
    }
}

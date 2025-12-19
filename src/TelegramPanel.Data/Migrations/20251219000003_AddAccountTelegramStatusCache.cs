using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramPanel.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20251219000003_AddAccountTelegramStatusCache")]
    public partial class AddAccountTelegramStatusCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelegramStatusSummary",
                table: "Accounts",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelegramStatusDetails",
                table: "Accounts",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TelegramStatusOk",
                table: "Accounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TelegramStatusCheckedAtUtc",
                table: "Accounts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramStatusSummary",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TelegramStatusDetails",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TelegramStatusOk",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TelegramStatusCheckedAtUtc",
                table: "Accounts");
        }
    }
}

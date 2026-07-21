using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhalletRoute.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFleetColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CapacityKg",
                table: "vehicles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CapacityM3",
                table: "vehicles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "drivers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseCategory",
                table: "drivers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LicenseExpiry",
                table: "drivers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "drivers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "drivers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacityKg",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "CapacityM3",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Document",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "LicenseCategory",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "LicenseExpiry",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "drivers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhalletRoute.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "deliveries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "deliveries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "deliveries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "deliveries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VolumeM3",
                table: "deliveries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WeightKg",
                table: "deliveries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "VolumeM3",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "deliveries");
        }
    }
}

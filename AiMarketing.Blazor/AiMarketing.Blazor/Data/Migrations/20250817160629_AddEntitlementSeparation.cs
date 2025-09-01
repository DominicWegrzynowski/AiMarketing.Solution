using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiMarketing.Blazor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitlementSeparation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentPeriodEndUtc",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentPeriodStartUtc",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntitlementTierNext",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostsUsedThisPeriod",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPeriodEndUtc",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentPeriodStartUtc",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EntitlementTierNext",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PostsUsedThisPeriod",
                table: "AspNetUsers");
        }
    }
}

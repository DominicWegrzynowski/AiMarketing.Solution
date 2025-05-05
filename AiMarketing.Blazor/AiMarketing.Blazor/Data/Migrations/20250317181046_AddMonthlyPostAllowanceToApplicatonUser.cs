using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiMarketing.Blazor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyPostAllowanceToApplicatonUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AllowanceEnd",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AllowanceStart",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MonthlyPostAllowance",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowanceEnd",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AllowanceStart",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MonthlyPostAllowance",
                table: "AspNetUsers");
        }
    }
}

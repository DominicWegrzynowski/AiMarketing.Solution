using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiMarketing.Blazor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeFieldsToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "Posts");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedWhen",
                table: "Posts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedWhen",
                table: "Posts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedWhen",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ModifiedWhen",
                table: "Posts");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledTime",
                table: "Posts",
                type: "datetime(6)",
                nullable: true);
        }
    }
}

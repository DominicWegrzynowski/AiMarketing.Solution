using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiMarketing.Blazor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeProductIdToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeProductId",
                table: "Products",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeProductId",
                table: "Products");
        }
    }
}

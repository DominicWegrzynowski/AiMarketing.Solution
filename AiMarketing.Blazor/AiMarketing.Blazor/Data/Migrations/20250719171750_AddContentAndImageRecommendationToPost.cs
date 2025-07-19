using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiMarketing.Blazor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContentAndImageRecommendationToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageRecommendation",
                table: "Posts",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageRecommendation",
                table: "Posts");
        }
    }
}

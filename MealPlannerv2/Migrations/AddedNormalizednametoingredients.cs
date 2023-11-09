using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealPlannerv2.Migrations
{
    /// <inheritdoc />
    public partial class AddedNormalizednametoingredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Ingredients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Ingredients");
        }
    }
}

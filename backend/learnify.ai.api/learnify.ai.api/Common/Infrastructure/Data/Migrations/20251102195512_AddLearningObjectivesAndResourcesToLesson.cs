using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learnify.ai.api.Common.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningObjectivesAndResourcesToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LearningObjectives",
                table: "Lessons",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resources",
                table: "Lessons",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearningObjectives",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Resources",
                table: "Lessons");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSkills_UserId",
                table: "UserSkills");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ReviewedUserId",
                table: "Reviews",
                newName: "IX_Review_ReviewedUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Skills",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkill_IsOffering_SkillId",
                table: "UserSkills",
                columns: new[] { "IsOffering", "SkillId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSkill_UserId_IsOffering",
                table: "UserSkills",
                columns: new[] { "UserId", "IsOffering" });

            migrationBuilder.CreateIndex(
                name: "IX_Skill_Category",
                table: "Skills",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSkill_IsOffering_SkillId",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_UserSkill_UserId_IsOffering",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_Skill_Category",
                table: "Skills");

            migrationBuilder.RenameIndex(
                name: "IX_Review_ReviewedUserId",
                table: "Reviews",
                newName: "IX_Reviews_ReviewedUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_UserId",
                table: "UserSkills",
                column: "UserId");
        }
    }
}

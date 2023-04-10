using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizPrepAi.Migrations
{
    public partial class _003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "UserAnswer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswer_QuestionId",
                table: "UserAnswer",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswer_Question_QuestionId",
                table: "UserAnswer",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswer_Question_QuestionId",
                table: "UserAnswer");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswer_QuestionId",
                table: "UserAnswer");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "UserAnswer");
        }
    }
}

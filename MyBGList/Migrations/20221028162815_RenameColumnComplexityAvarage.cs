using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBGList.Migrations
{
    public partial class RenameColumnComplexityAvarage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ComplexityAvarage",
                table: "BoardGames",
                newName: "ComplexityAverage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ComplexityAverage",
                table: "BoardGames",
                newName: "ComplexityAvarage");
        }
    }
}

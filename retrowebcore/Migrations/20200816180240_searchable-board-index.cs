using Microsoft.EntityFrameworkCore.Migrations;

namespace retrowebcore.Migrations
{
    public partial class searchableboardindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_boards_search_vector",
                table: "boards",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_boards_search_vector",
                table: "boards");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace retrowebcore.Migrations
{
    public partial class searchable_board : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "boards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "boards");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace retrowebcore.Migrations
{
    public partial class searchableboardcontent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "search_content",
                table: "boards",
                nullable: true);

            migrationBuilder.Sql(
                @"CREATE TRIGGER board_search_vector_update BEFORE INSERT OR UPDATE
                  ON boards FOR EACH ROW EXECUTE PROCEDURE
                  tsvector_update_trigger(search_vector, 'pg_catalog.english', search_content);"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "search_content",
                table: "boards");

            migrationBuilder.Sql("DROP TRIGGER board_search_vector_update");
        }
    }
}

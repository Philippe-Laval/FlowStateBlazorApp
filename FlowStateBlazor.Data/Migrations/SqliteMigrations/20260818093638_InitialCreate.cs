using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowStateBlazor.Data.Migrations.SqliteMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FLOW_FLOWGRAPH_DESCRIPTION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NAME = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    JSON_SERIALIZED_FLOW = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLOW_FLOWGRAPH_DESCRIPTION", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FLOW_FLOWGRAPH_DESCRIPTION");
        }
    }
}

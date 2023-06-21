using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shooping.Migrations
{
    /// <inheritdoc />
    public partial class AñadirIndexNameStateId_en_City : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_StateId",
                table: "Cities",
                columns: new[] { "Name", "StateId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_Name_StateId",
                table: "Cities");
        }
    }
}

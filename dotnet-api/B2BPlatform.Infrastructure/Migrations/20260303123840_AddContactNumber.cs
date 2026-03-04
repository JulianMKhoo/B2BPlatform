using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2BPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "contact_number",
                table: "Company",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact_number",
                table: "Company");
        }
    }
}

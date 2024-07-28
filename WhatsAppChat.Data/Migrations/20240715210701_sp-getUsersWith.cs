using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spgetUsersWith : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getUsersWith] @id INT
                AS
                BEGIN
	                SELECT * FROM Users WHERE Id = @id;
                END;";
            migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

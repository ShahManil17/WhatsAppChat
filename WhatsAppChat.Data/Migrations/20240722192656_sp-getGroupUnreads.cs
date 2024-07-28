using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spgetGroupUnreads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getGroupUnreads]
            @senderId INT, @groupId VARCHAR(MAX)
            AS
            BEGIN
	            SELECT COUNT(*) AS Count, @groupId AS GroupId FROM GroupUnreads WHERE UserId = @senderId AND GroupId = @groupId;
            END;";
			migrationBuilder.Sql(sp);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

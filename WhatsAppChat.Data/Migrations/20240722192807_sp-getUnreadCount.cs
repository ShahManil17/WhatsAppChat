using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spgetUnreadCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getUnreadCount]
            @senderId INT, @receiverId INT 
            AS
            BEGIN
	            SELECT COUNT(*) AS Count, @senderId AS SenderId FROM Communication WHERE SenderId = @senderId AND ReceiverId = @receiverId AND IsRead = 0;
            END;";
			migrationBuilder.Sql(sp);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

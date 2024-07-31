using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spUpdatedgetUsersExcept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getUsersExcept] @id INT
            AS
            BEGIN
	            SELECT u.*,
	            (
		            SELECT TOP(1) [Message] FROM Communication
		            WHERE (Communication.SenderId = @id AND Communication.ReceiverId = u.Id) OR (Communication.SenderId = u.Id AND Communication.ReceiverId = @id) ORDER BY Id DESC
	            ) AS LastMessage 
	            FROM Users AS u WHERE u.Id != @id;
            END;";
			migrationBuilder.Sql(sp);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

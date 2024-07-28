using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spgetUserChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE OR ALTER   PROCEDURE [dbo].[getUserChat]
			@senderId INT, @receiverId INT
			AS
			BEGIN
				SELECT (
				SELECT Users.Id, Users.ProfileImage, Users.UserName, Users.LogoutTime,
				(
					SELECT message.* FROM Communication AS message
					WHERE (message.SenderId = @senderId AND ReceiverId = @receiverId) OR (message.ReceiverId = @senderId AND SenderId = @receiverId)
					FOR JSON PATH
				)AS Message
				FROM Users AS Users WHERE Users.Id = @receiverId
				FOR JSON PATH
				 )
			END;";
			migrationBuilder.Sql(sp);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

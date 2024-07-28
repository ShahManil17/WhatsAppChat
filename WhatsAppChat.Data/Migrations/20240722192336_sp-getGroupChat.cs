using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spgetGroupChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getGroupChat]
			@groupId VARCHAR(max)
			AS
			BEGIN
				SELECT (
					SELECT Groups.Id, Groups.GroupIcon, Groups.GroupName,
					(
						SELECT message.*, Users.UserName FROM GroupMessages AS message
						INNER JOIN Users ON Users.Id = message.SenderId
						WHERE (message.GroupId = @groupId)
						FOR JSON PATH
					)AS Message
					FROM Groups AS Groups WHERE Groups.Id = @groupId
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

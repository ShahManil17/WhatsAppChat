using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spgetGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getGroups]
			@id INT
			AS
			BEGIN
				SELECT Groups.Id, Groups.GroupIcon, Groups.GroupName, 
				(
					SELECT TOP (1) Message FROM GroupMessages AS messages
					WHERE messages.GroupId = Groups.Id
					ORDER BY SendTime DESC
				) AS LastMessage
				FROM Groups
				INNER JOIN GroupHasMembers AS members ON members.GroupId = Groups.Id
				WHERE members.UserId = @id;
			END;";
			migrationBuilder.Sql(sp);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

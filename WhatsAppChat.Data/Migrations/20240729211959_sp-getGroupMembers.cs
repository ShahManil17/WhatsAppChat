using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class spgetGroupMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getGroupMembers] @groupId VARCHAR(MAX)
            AS
            BEGIN
	            SELECT Users.UserName, Users.Id FROM Groups
	            INNER JOIN GroupHasMembers AS Members ON Members.GroupId = Groups.Id
	            INNER JOIN Users ON Users.Id = Members.UserId
	            WHERE Groups.Id = @groupId
            END;";
			migrationBuilder.Sql(sp);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

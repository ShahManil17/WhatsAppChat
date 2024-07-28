using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class PrimaryKey_added_WithDiffrent_Datatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "GroupMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "GroupHasMembers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessages_GroupId",
                table: "GroupMessages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupHasMembers_GroupId",
                table: "GroupHasMembers",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupHasMembers_Groups_GroupId",
                table: "GroupHasMembers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessages_Groups_GroupId",
                table: "GroupMessages",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupHasMembers_Groups_GroupId",
                table: "GroupHasMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessages_Groups_GroupId",
                table: "GroupMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_GroupMessages_GroupId",
                table: "GroupMessages");

            migrationBuilder.DropIndex(
                name: "IX_GroupHasMembers_GroupId",
                table: "GroupHasMembers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GroupMessages");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GroupHasMembers");
        }
    }
}

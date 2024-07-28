using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class PrimaryKey_of_Groups_Removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "GroupMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "GroupHasMembers",
                type: "int",
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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class UnreadMessages_table_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupUnreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUnreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupUnreads_GroupMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "GroupMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupUnreads_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupUnreads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupUnreads_GroupId",
                table: "GroupUnreads",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUnreads_MessageId",
                table: "GroupUnreads",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUnreads_UserId",
                table: "GroupUnreads",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupUnreads");
        }
    }
}

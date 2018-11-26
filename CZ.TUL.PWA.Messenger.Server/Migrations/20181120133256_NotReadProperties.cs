using Microsoft.EntityFrameworkCore.Migrations;

namespace CZ.TUL.PWA.Messenger.Server.Migrations
{
    public partial class NotReadProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConversation_Conversations_ConversationId",
                table: "UserConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversation_AspNetUsers_UserId",
                table: "UserConversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConversation",
                table: "UserConversation");

            migrationBuilder.RenameTable(
                name: "UserConversation",
                newName: "UserConversations");

            migrationBuilder.DropIndex(
                name: "IX_UserConversation_UserId",
                table: "UserConversations");

            migrationBuilder.CreateIndex(
                name: "IX_UserConversations_UserId",
                table: "UserConversations",
                column: "UserId");

            migrationBuilder.AddColumn<bool>(
                name: "NotRead",
                table: "UserConversations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NotReadCount",
                table: "UserConversations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConversations",
                table: "UserConversations",
                columns: new[] { "ConversationId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversations_AspNetUsers_UserId",
                table: "UserConversations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversations_AspNetUsers_UserId",
                table: "UserConversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConversations",
                table: "UserConversations");

            migrationBuilder.DropColumn(
                name: "NotRead",
                table: "UserConversations");

            migrationBuilder.DropColumn(
                name: "NotReadCount",
                table: "UserConversations");

            migrationBuilder.RenameTable(
                name: "UserConversations",
                newName: "UserConversation");

            migrationBuilder.DropIndex(
                name: "IX_UserConversations_UserId",
                table: "UserConversation");

            migrationBuilder.CreateIndex(
                name: "IX_UserConversation_UserId",
                table: "UserConversation",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConversation",
                table: "UserConversation",
                columns: new[] { "ConversationId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversation_Conversations_ConversationId",
                table: "UserConversation",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversation_AspNetUsers_UserId",
                table: "UserConversation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

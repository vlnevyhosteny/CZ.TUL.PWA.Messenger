using Microsoft.EntityFrameworkCore.Migrations;

namespace CZ.TUL.PWA.Messenger.Server.Migrations
{
    public partial class AutoIncrementsOnColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .Sql("ALTER TABLE `Messages` CHANGE `MessageId` `MessageId` INT(11) NOT NULL AUTO_INCREMENT;");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder
                .Sql("ALTER TABLE `Conversations` CHANGE `ConversationId` `ConversationId` INT(11) NOT NULL AUTO_INCREMENT;");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .Sql("ALTER TABLE `Messages` CHANGE `MessageId` `MessageId` INT(11) NOT NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder
                .Sql("ALTER TABLE `Conversations` CHANGE `ConversationId` `ConversationId` INT(11) NOT NULL;");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace CZ.TUL.PWA.Messenger.Server.Migrations
{
    public partial class HubConnectionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HubConnections",
                columns: table => new
                {
                    HubConnectionId = table.Column<string>(nullable: false),
                    UserAnget = table.Column<string>(nullable: true),
                    Connected = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubConnections", x => x.HubConnectionId);
                    table.ForeignKey(
                        name: "FK_HubConnections_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HubConnections_UserId",
                table: "HubConnections",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HubConnections");
        }
    }
}

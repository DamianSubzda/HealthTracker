using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HealthTracker.Server.Migrations
{
    /// <inheritdoc />
    public partial class FriendshipEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_User1Id",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_User2Id",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_Status_StatusId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Status_StatusId",
                table: "Notification");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropIndex(
                name: "IX_Notification_StatusId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_StatusId",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Friendship");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "Friendship",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "User1Id",
                table: "Friendship",
                newName: "FriendId");

            migrationBuilder.RenameColumn(
                name: "DateOfStart",
                table: "Friendship",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_User2Id",
                table: "Friendship",
                newName: "IX_Friendship_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_User1Id",
                table: "Friendship",
                newName: "IX_Friendship_FriendId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notification",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Friendship",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Friendship",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_FriendId",
                table: "Friendship",
                column: "FriendId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_UserId",
                table: "Friendship",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_FriendId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_UserId",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Friendship");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Friendship",
                newName: "User2Id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Friendship",
                newName: "DateOfStart");

            migrationBuilder.RenameColumn(
                name: "FriendId",
                table: "Friendship",
                newName: "User1Id");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_UserId",
                table: "Friendship",
                newName: "IX_Friendship_User2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_FriendId",
                table: "Friendship",
                newName: "IX_Friendship_User1Id");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Friendship",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_StatusId",
                table: "Notification",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_StatusId",
                table: "Friendship",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Name",
                table: "Status",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_User1Id",
                table: "Friendship",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_User2Id",
                table: "Friendship",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_Status_StatusId",
                table: "Friendship",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Status_StatusId",
                table: "Notification",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

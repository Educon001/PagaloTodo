using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCABPagaloTodoMS.Infrastructure.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class JorgeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: new Guid("0babfd05-81dc-43fb-b189-c1e9b094e0d1"));

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "PasswordHash", "Status", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { new Guid("15fc1c8a-aeb9-4dd3-a3ba-d2fb0d4e9870"), new DateTime(2023, 7, 16, 12, 8, 39, 960, DateTimeKind.Utc).AddTicks(664), "APP", "pagalotodoucabaf@gmail.com", false, "admin", "$PagalTodo$10000$u1opoYuYxSOdhIG4y7IbtIu9TdbwpLAyBfrsC4maqgJW2XP9", true, null, null, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: new Guid("15fc1c8a-aeb9-4dd3-a3ba-d2fb0d4e9870"));

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "PasswordHash", "Status", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { new Guid("0babfd05-81dc-43fb-b189-c1e9b094e0d1"), new DateTime(2023, 7, 15, 2, 4, 53, 131, DateTimeKind.Utc).AddTicks(2193), "APP", "pagalotodoucabaf@gmail.com", false, "admin", "$PagalTodo$10000$tGje5e0uqjA0bLGFajGTIV8kH+3vtsTG2wWnML7Zu3LeuBT5", true, null, null, "admin" });
        }
    }
}

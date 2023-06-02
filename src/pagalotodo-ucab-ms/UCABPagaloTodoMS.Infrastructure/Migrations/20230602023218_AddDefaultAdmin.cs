using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCABPagaloTodoMS.Infrastructure.Migrations
{
    public partial class AddDefaultAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "PasswordHash", "Status", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { new Guid("fb5057f0-ea5a-4516-be3e-51d44b020484"), new DateTime(2023, 6, 2, 2, 32, 18, 319, DateTimeKind.Utc).AddTicks(4618), "APP", "pagalotodoucabaf@gmail.com", false, "admin", "$PagalTodo$10000$T9BHdCiMxXWXprh/aH/A19BihGQsjrvu2i+hjr77Kkmb0Qsb", true, null, null, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: new Guid("fb5057f0-ea5a-4516-be3e-51d44b020484"));
        }
    }
}

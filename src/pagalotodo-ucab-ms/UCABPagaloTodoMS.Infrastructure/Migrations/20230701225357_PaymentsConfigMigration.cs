using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCABPagaloTodoMS.Infrastructure.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class PaymentsConfigMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDetailEntity_Payments_PaymentId",
                table: "PaymentDetailEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentDetailEntity",
                table: "PaymentDetailEntity");

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: new Guid("4ef5592b-25f3-4cf8-a0e4-9b30c051f9bf"));

            migrationBuilder.RenameTable(
                name: "PaymentDetailEntity",
                newName: "PaymentDetails");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentDetailEntity_PaymentId",
                table: "PaymentDetails",
                newName: "IX_PaymentDetails_PaymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentDetails",
                table: "PaymentDetails",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PaymentFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentFields_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "PasswordHash", "Status", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { new Guid("fb91d3d4-a257-4eae-b654-c16b37b0524a"), new DateTime(2023, 7, 1, 22, 53, 57, 435, DateTimeKind.Utc).AddTicks(4606), "APP", "pagalotodoucabaf@gmail.com", false, "admin", "$PagalTodo$10000$BBfBi2iwnD9YBHx2gmIrxv00H6U+evcHZtjVpzVkZSIaARHk", true, null, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentFields_ServiceId",
                table: "PaymentFields",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDetails_Payments_PaymentId",
                table: "PaymentDetails",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDetails_Payments_PaymentId",
                table: "PaymentDetails");

            migrationBuilder.DropTable(
                name: "PaymentFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentDetails",
                table: "PaymentDetails");

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: new Guid("fb91d3d4-a257-4eae-b654-c16b37b0524a"));

            migrationBuilder.RenameTable(
                name: "PaymentDetails",
                newName: "PaymentDetailEntity");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentDetails_PaymentId",
                table: "PaymentDetailEntity",
                newName: "IX_PaymentDetailEntity_PaymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentDetailEntity",
                table: "PaymentDetailEntity",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "PasswordHash", "Status", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { new Guid("4ef5592b-25f3-4cf8-a0e4-9b30c051f9bf"), new DateTime(2023, 7, 1, 22, 48, 26, 681, DateTimeKind.Utc).AddTicks(2342), "APP", "pagalotodoucabaf@gmail.com", false, "admin", "$PagalTodo$10000$yLkBFcM0PdP+IUBVRd+Wj/dBxTUcQZXkepeqf9sbAN8n53F9", true, null, null, "admin" });

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDetailEntity_Payments_PaymentId",
                table: "PaymentDetailEntity",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");
        }
    }
}

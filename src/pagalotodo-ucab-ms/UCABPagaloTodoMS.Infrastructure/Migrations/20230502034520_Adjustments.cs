using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCABPagaloTodoMS.Infrastructure.Migrations
{
    public partial class Adjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debtors_Services_ServiceEntityId",
                table: "Debtors");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Services_ServiceEntityId",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Consumers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Providers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "ServiceEntityId",
                table: "Fields",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Fields_ServiceEntityId",
                table: "Fields",
                newName: "IX_Fields_ServiceId");

            migrationBuilder.RenameColumn(
                name: "ServiceEntityId",
                table: "Debtors",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Debtors_ServiceEntityId",
                table: "Debtors",
                newName: "IX_Debtors_ServiceId");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Consumers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Admins",
                newName: "PasswordHash");

            migrationBuilder.AddForeignKey(
                name: "FK_Debtors_Services_ServiceId",
                table: "Debtors",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Services_ServiceId",
                table: "Fields",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debtors_Services_ServiceId",
                table: "Debtors");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Services_ServiceId",
                table: "Fields");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Providers",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Fields",
                newName: "ServiceEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Fields_ServiceId",
                table: "Fields",
                newName: "IX_Fields_ServiceEntityId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Debtors",
                newName: "ServiceEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Debtors_ServiceId",
                table: "Debtors",
                newName: "IX_Debtors_ServiceEntityId");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Consumers",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Admins",
                newName: "Token");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Consumers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Debtors_Services_ServiceEntityId",
                table: "Debtors",
                column: "ServiceEntityId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Services_ServiceEntityId",
                table: "Fields",
                column: "ServiceEntityId",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}

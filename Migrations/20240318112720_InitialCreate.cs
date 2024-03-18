using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndividualNorthwindEshop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
            migrationBuilder.AddColumn<int>(
    name: "UserId",
    table: "Customers",
    nullable: true);

            migrationBuilder.CreateIndex(
          name: "IX_Customers_UserId",
          table: "Customers",
          column: "UserId")
          .Annotation("SqlServer:Filter", "[UserId] IS NOT NULL");



            migrationBuilder.AddColumn<int>(
       name: "UserId",
       table: "Employees",
       nullable: true);
            migrationBuilder.CreateIndex(
      name: "IX_Employees_UserId",
      table: "Employees",
      column: "UserId")
      .Annotation("SqlServer:Filter", "[UserId] IS NOT NULL");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
            name: "IX_Customers_UserId",
            table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

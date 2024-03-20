using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndividualNorthwindEshop.Migrations.UserRelationships
{
    /// <inheritdoc />
    public partial class AddCartAndCartItemTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                 name: "Carts",
                 columns: table => new
                 {
                     CartId = table.Column<int>(nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     CustomerId = table.Column<string>(nullable: true)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Carts", x => x.CartId);
                     table.ForeignKey(
                         name: "FK_Carts_Customers_CustomerId",
                         column: x => x.CustomerId,
                         principalTable: "Customers",
                         principalColumn: "CustomerId",
                         onDelete: ReferentialAction.Restrict);
                 });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
              name: "CartItems");

            migrationBuilder.DropTable(
                name: "Carts");

        }
    }
}


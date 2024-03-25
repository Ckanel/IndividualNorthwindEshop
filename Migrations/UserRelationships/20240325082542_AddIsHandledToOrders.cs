using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndividualNorthwindEshop.Migrations.UserRelationships
{
    /// <inheritdoc />
    public partial class AddIsHandledToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsHandled",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHandled",
                table: "Orders");

           
        }
    }
}

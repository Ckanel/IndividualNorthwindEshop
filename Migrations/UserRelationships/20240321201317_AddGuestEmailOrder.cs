using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndividualNorthwindEshop.Migrations.UserRelationships
{
    /// <inheritdoc />
    public partial class AddGuestEmailOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
     name: "GuestEmail",
     table: "Orders",
     maxLength: 100,
     nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}

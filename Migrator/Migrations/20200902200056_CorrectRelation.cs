using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrator.Migrations
{
    public partial class CorrectRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DishInOrder",
                table: "DishInOrder");

            migrationBuilder.RenameTable(
                name: "DishInOrder",
                newName: "DishInOrders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishInOrders",
                table: "DishInOrders",
                columns: new[] { "DishId", "OrderId" });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishInOrders_OrderId",
                table: "DishInOrders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishInOrders_Dishes_DishId",
                table: "DishInOrders",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishInOrders_Orders_OrderId",
                table: "DishInOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishInOrders_Dishes_DishId",
                table: "DishInOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DishInOrders_Orders_OrderId",
                table: "DishInOrders");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishInOrders",
                table: "DishInOrders");

            migrationBuilder.DropIndex(
                name: "IX_DishInOrders_OrderId",
                table: "DishInOrders");

            migrationBuilder.RenameTable(
                name: "DishInOrders",
                newName: "DishInOrder");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishInOrder",
                table: "DishInOrder",
                columns: new[] { "DishId", "OrderId" });
        }
    }
}

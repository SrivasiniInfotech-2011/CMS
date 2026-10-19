using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitiMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CRM_Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dob = table.Column<DateTime>(type: "DateTime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRM_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CRM_Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "DateTime", nullable: false),
                    OrderValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRM_Orders", x => x.Id);
                    table.CheckConstraint("CHK_CRM_Orders_OrderStatus", "OrderStatus IN (4, 1, 2, 0, 3)");
                    table.ForeignKey(
                        name: "FK_CRM_Orders_CRM_Customers",
                        column: x => x.CustomerId,
                        principalTable: "CRM_Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRM_OrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemSku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRM_OrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_CRM_Orders_OrderDetails",
                        column: x => x.OrderId,
                        principalTable: "CRM_Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CRM_OrderDetails_OrderId",
                table: "CRM_OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CRM_Orders_CustomerId",
                table: "CRM_Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRM_OrderDetails");

            migrationBuilder.DropTable(
                name: "CRM_Orders");

            migrationBuilder.DropTable(
                name: "CRM_Customers");
        }
    }
}
